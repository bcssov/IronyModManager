using System;
using System.Collections.Generic;
using System.Text;
using IronyModManager.DI;
using IronyModManager.Shared;
using Moq;
using SimpleInjector;

namespace IronyModManager.Tests.Common
{
    public class DIContainer
    {
        public static void SetupMockType(params Type[] types)
        {
            var container = new Mock<Container>();
            foreach (var item in types)
            {
                container.Setup(p => p.GetInstance(item)).Returns()
            }            
        }
    }
}
