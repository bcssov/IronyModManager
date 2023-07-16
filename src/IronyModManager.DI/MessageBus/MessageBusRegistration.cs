
// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 06-10-2020
//
// Last Modified By : Mario
// Last Modified On : 06-26-2023
// ***********************************************************************
// <copyright file="MessageBusRegistration.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IronyModManager.Shared.MessageBus;
using SlimMessageBus.Host;
using SlimMessageBus.Host.Interceptor;

namespace IronyModManager.DI.MessageBus
{

    /// <summary>
    /// Class MessageBusRegistration.
    /// </summary>
    internal static class MessageBusRegistration
    {
        #region Methods

        /// <summary>
        /// Registers the specified assemblies.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        public static void Register(IEnumerable<Assembly> assemblies)
        {
            static object initCollectionObject(Type type)
            {
                var listType = typeof(List<>);
                var genericListType = listType.MakeGenericType(type);
                var instance = Activator.CreateInstance(genericListType);
                return instance;
            }
            static Type resolveProducerInterceptor(Type type)
            {
                return typeof(IProducerInterceptor<>).MakeGenericType(type);
            }
            static Type resolvePublishInterceptor(Type type)
            {
                return typeof(IPublishInterceptor<>).MakeGenericType(type);
            }
            static Type resolveConsumerInterceptor(Type type)
            {
                return typeof(IConsumerInterceptor<>).MakeGenericType(type);
            }
            static Type resolveEnumerable(Type type)
            {
                return typeof(IEnumerable<>).MakeGenericType(type);
            }

            var registeredTypes = new Dictionary<Type, string>();
            var registeredCollections = new Dictionary<Type, Func<object>>();
            var builder = MessageBusBuilder.Create()
                .WithDependencyResolver(new MessageBusDependencyResolver(registeredCollections, new MessageTypeResolver(registeredTypes)))
                .WithProvider(providerSettings => new MessageBusMemoryProvider(providerSettings))
                .Do(builder =>
                {
                    assemblies.ToList().ForEach(assembly =>
                    {
                        assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract)
                            .SelectMany(t => t.GetInterfaces(), (t, i) => new { Type = t, Interface = i })
                            .Where(x => x.Interface.IsGenericType && x.Interface.GetGenericTypeDefinition() == typeof(IMessageBusConsumer<>))
                            .Select(x => new { HandlerType = x.Type, EventType = x.Interface.GetGenericArguments()[0] })
                            .ToList()
                            .ForEach(find =>
                            {
                                // Yeah, samples are really generic. I could not be more sarcastic about this.
                                if (!registeredTypes.ContainsKey(find.EventType))
                                {
                                    registeredTypes.Add(find.EventType, find.EventType.Name);
                                    builder.Produce(find.EventType, x => x.DefaultTopic(x.Settings.MessageType.Name));
                                    var producerType = resolveProducerInterceptor(find.EventType);
                                    var publishType = resolvePublishInterceptor(find.EventType);
                                    var consumerType = resolveConsumerInterceptor(find.EventType);
                                    var resolvedProducerType = resolveEnumerable(producerType);
                                    Func<object> initResolvedProducerType = () => initCollectionObject(producerType);
                                    var resolvedPublishType = resolveEnumerable(publishType);
                                    Func<object> initResolvedPublishType = () => initCollectionObject(publishType);
                                    var resolvedConsumerType = resolveEnumerable(consumerType);
                                    Func<object> initResolvedConsumerType = () => initCollectionObject(consumerType);
                                    DIContainer.Container.Register(resolvedProducerType, initResolvedProducerType);
                                    registeredCollections.Add(resolvedProducerType, initResolvedProducerType);
                                    DIContainer.Container.Register(resolvedPublishType, initResolvedPublishType);
                                    registeredCollections.Add(resolvedPublishType, initResolvedPublishType);
                                    DIContainer.Container.Register(resolvedConsumerType, initResolvedConsumerType);
                                    registeredCollections.Add(resolvedConsumerType, initResolvedConsumerType);
                                }
                                builder.Consume(find.EventType, x => x.Topic(x.ConsumerSettings.MessageType.Name).WithConsumer(find.HandlerType));
                                DIContainer.Container.Register(find.HandlerType, find.HandlerType, SimpleInjector.Lifestyle.Singleton);
                            });
                    });
                });
            var mbus = new MessageBus(builder.Build(), registeredTypes.Keys.ToHashSet());
            DIContainer.Container.RegisterInstance<IMessageBus>(mbus);
        }

        #endregion Methods
    }
}
