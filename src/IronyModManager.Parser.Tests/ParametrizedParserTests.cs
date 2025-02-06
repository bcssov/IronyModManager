// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 10-03-2023
//
// Last Modified By : Mario
// Last Modified On : 02-06-2025
// ***********************************************************************
// <copyright file="ParametrizedParserTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using IronyModManager.DI;
using IronyModManager.Localization;
using IronyModManager.Parser.Common;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Shared;
using IronyModManager.Tests.Common;
using Xunit;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class ParametrizedParserTests.
    /// </summary>
    public class ParametrizedParserTests
    {
        /// <summary>
        /// Defines the test method GetObjectId_should_yield_results.
        /// </summary>
        [Fact]
        public void GetObjectId_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder(3175);
            sb.AppendLine(@"building_giga_megaworkshop_hub_acot_$tier$ = {");
            sb.AppendLine(@"	base_buildtime = @giga_amb_hub_time_$tier$");
            sb.AppendLine(@"	category = manufacturing");
            sb.AppendLine(@"	icon = ""acot_compat/building_giga_megaworkshop_hub_acot_$tier$""");
            sb.AppendLine(@"");
            sb.AppendLine(@"	can_build = no # upgrade");
            sb.AppendLine(@"");
            sb.AppendLine(@"	resources = {");
            sb.AppendLine(@"		category = planet_buildings");
            sb.AppendLine(@"		cost = {");
            sb.AppendLine(@"			minerals = @giga_amb_hub_cost_minerals_$tier$");
            sb.AppendLine(@"			$resource1$ = @giga_amb_hub_cost_resource1_$tier$");
            sb.AppendLine(@"			$resource2$ = @giga_amb_hub_cost_resource2_$tier$");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		upkeep = {");
            sb.AppendLine(@"			$resource1$ = @giga_amb_hub_upkeep_$tier$");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"");
            sb.AppendLine(@"		# auto production");
            sb.AppendLine(@"		upkeep = {");
            sb.AppendLine(@"			trigger = {");
            sb.AppendLine(@"				has_modifier = acot_modifier_ascended_city");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"");
            sb.AppendLine(@"			alloys = @giga_amb_megaengineer_$tier$_alloys_upkeep");
            sb.AppendLine(@"			unity = @giga_amb_megaengineer_$tier$_unity_upkeep");
            sb.AppendLine(@"			$resource1$ = @giga_amb_megaengineer_$tier$_resource1_upkeep");
            sb.AppendLine(@"			$resource2$ = @giga_amb_megaengineer_$tier$_resource2_upkeep");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		produces = {");
            sb.AppendLine(@"			trigger = {");
            sb.AppendLine(@"				has_modifier = acot_modifier_ascended_city");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"			giga_sr_amb_megaconstruction = @giga_amb_megaengineer_$tier$_output");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	prerequisites = {");
            sb.AppendLine(@"		giga_tech_amb_supertensiles_acot_$tier$");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	potential = {");
            sb.AppendLine(@"		nor = { ");
            sb.AppendLine(@"			has_global_flag = acot_building_forbidden");
            sb.AppendLine(@"			has_modifier = resort_colony");
            sb.AppendLine(@"			has_modifier = slave_colony ");
            sb.AppendLine(@"			has_modifier = crucible_colony");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		has_global_flag = @giga_amb_flag");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	allow = {");
            sb.AppendLine(@"		$allow$");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	# they're all upgrades, so this isn't necessary (and in fact breaks things)");
            sb.AppendLine(@"	# empire_limit = {");
            sb.AppendLine(@"	# 	base = 0");
            sb.AppendLine(@"	# 	modifier = {");
            sb.AppendLine(@"	# 		is_variable_set = giga_amb_cap");
            sb.AppendLine(@"	# 		add = giga_amb_cap");
            sb.AppendLine(@"	# 	}");
            sb.AppendLine(@"	# }");
            sb.AppendLine(@"");
            sb.AppendLine(@"	destroy_trigger = {");
            sb.AppendLine(@"		OR = {");
            sb.AppendLine(@"			has_modifier = resort_colony");
            sb.AppendLine(@"			has_modifier = slave_colony");
            sb.AppendLine(@"			has_modifier = crucible_colony");
            sb.AppendLine(@"			NOT = { has_global_flag = @giga_amb_flag }");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	upgrades = {");
            sb.AppendLine(@"		$upgrade$");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	country_modifier = {");
            sb.AppendLine(@"		country_resource_max_giga_sr_amb_megaconstruction_add = @giga_amb_hub_storage_$tier$");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	triggered_planet_modifier = {");
            sb.AppendLine(@"		potential = {");
            sb.AppendLine(@"			exists = owner");
            sb.AppendLine(@"			owner = {");
            sb.AppendLine(@"				is_gestalt = yes");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		modifier = {");
            sb.AppendLine(@"			job_giga_megaengineering_overseer_drone_acot_$tier$_add = 2");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	triggered_planet_modifier = {");
            sb.AppendLine(@"		potential = {");
            sb.AppendLine(@"			exists = owner");
            sb.AppendLine(@"			owner = {");
            sb.AppendLine(@"				is_gestalt = no");
            sb.AppendLine(@"				#is_fallen_empire_spiritualist = no");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		modifier = {");
            sb.AppendLine(@"			job_giga_megaengineering_overseer_acot_$tier$_add = 2");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	# triggered_planet_modifier = {");
            sb.AppendLine(@"	# 	potential = {");
            sb.AppendLine(@"	# 		exists = owner");
            sb.AppendLine(@"	# 		owner = {");
            sb.AppendLine(@"	# 			is_fallen_empire_spiritualist = yes");
            sb.AppendLine(@"	# 		}");
            sb.AppendLine(@"	# 	}");
            sb.AppendLine(@"	# 	modifier = {");
            sb.AppendLine(@"	# 		job_giga_amb_fe_celestial_architect_add = 2");
            sb.AppendLine(@"	# 	}");
            sb.AppendLine(@"	# }");
            sb.AppendLine(@"");
            sb.AppendLine(@"	triggered_desc = {");
            sb.AppendLine(@"		trigger = {");
            sb.AppendLine(@"			exists = owner");
            sb.AppendLine(@"			owner = {");
            sb.AppendLine(@"				is_gestalt = no");
            sb.AppendLine(@"				#is_fallen_empire_spiritualist = no");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		text = job_giga_megaengineering_overseer_acot_$tier$_effect_desc");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	triggered_desc = {");
            sb.AppendLine(@"		trigger = {");
            sb.AppendLine(@"			exists = owner");
            sb.AppendLine(@"			owner = {");
            sb.AppendLine(@"				is_gestalt = yes");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		text = job_giga_megaengineering_overseer_drone_acot_$tier$_effect_desc");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	# triggered_desc = {");
            sb.AppendLine(@"	# 	trigger = {");
            sb.AppendLine(@"	# 		exists = owner");
            sb.AppendLine(@"	# 		owner = {");
            sb.AppendLine(@"	# 			is_fallen_empire_spiritualist = yes");
            sb.AppendLine(@"	# 		}");
            sb.AppendLine(@"	# 	}");
            sb.AppendLine(@"	# 	text = job_giga_amb_fe_celestial_architect_effect_desc");
            sb.AppendLine(@"	# }");
            sb.AppendLine(@"}");


            var sb2 = new StringBuilder(257);
            sb2.AppendLine(@"inline_script = {");
            sb2.AppendLine(@"	script = ""buildings/building_giga_megaworkshop_acot""");
            sb2.AppendLine(@"	tier = ""delta""");
            sb2.AppendLine(@"");
            sb2.AppendLine(@"	allow = ""has_enigmatic_capital = yes""");
            sb2.AppendLine(@"	upgrade = ""building_giga_megaworkshop_acot_alpha""");
            sb2.AppendLine(@"");
            sb2.AppendLine(@"	resource1 = ""sr_dark_matter""");
            sb2.AppendLine(@"	resource2 = ""acot_sr_dark_energy""");
            sb2.AppendLine(@"}");

            var parser = new ParametrizedParser(new CodeParser(new Logger()));
            var result = parser.Process(sb.ToString(), sb2.ToString());
            result.Should().NotBeNullOrEmpty();
            var m = DIResolver.Get<IParserManager>();
            var parserResult = m.Parse(new ParserManagerArgs { File = "common\\buildings\\dummy.txt", GameType = "Stellaris", IsBinary = false, Lines = result.SplitOnNewLine() });
            parserResult.Count().Should().Be(1);
            parserResult.FirstOrDefault().Id.Should().Be("building_giga_megaworkshop_hub_acot_delta");
        }

        /// <summary>
        /// Defines the test method GetObjectId_should_yield_results_without_using_parameters.
        /// </summary>
        [Fact]
        public void GetObjectId_should_yield_results_without_using_parameters()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder(3175);
            sb.AppendLine(@"building_giga_megaworkshop_hub_acot_no_parameters = {");
            sb.AppendLine(@"	base_buildtime = @giga_amb_hub_time_$tier$");
            sb.AppendLine(@"	category = manufacturing");
            sb.AppendLine(@"	icon = ""acot_compat/building_giga_megaworkshop_hub_acot_$tier$""");
            sb.AppendLine(@"");
            sb.AppendLine(@"	can_build = no # upgrade");
            sb.AppendLine(@"");
            sb.AppendLine(@"	resources = {");
            sb.AppendLine(@"		category = planet_buildings");
            sb.AppendLine(@"		cost = {");
            sb.AppendLine(@"			minerals = @giga_amb_hub_cost_minerals_$tier$");
            sb.AppendLine(@"			$resource1$ = @giga_amb_hub_cost_resource1_$tier$");
            sb.AppendLine(@"			$resource2$ = @giga_amb_hub_cost_resource2_$tier$");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		upkeep = {");
            sb.AppendLine(@"			$resource1$ = @giga_amb_hub_upkeep_$tier$");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"");
            sb.AppendLine(@"		# auto production");
            sb.AppendLine(@"		upkeep = {");
            sb.AppendLine(@"			trigger = {");
            sb.AppendLine(@"				has_modifier = acot_modifier_ascended_city");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"");
            sb.AppendLine(@"			alloys = @giga_amb_megaengineer_$tier$_alloys_upkeep");
            sb.AppendLine(@"			unity = @giga_amb_megaengineer_$tier$_unity_upkeep");
            sb.AppendLine(@"			$resource1$ = @giga_amb_megaengineer_$tier$_resource1_upkeep");
            sb.AppendLine(@"			$resource2$ = @giga_amb_megaengineer_$tier$_resource2_upkeep");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		produces = {");
            sb.AppendLine(@"			trigger = {");
            sb.AppendLine(@"				has_modifier = acot_modifier_ascended_city");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"			giga_sr_amb_megaconstruction = @giga_amb_megaengineer_$tier$_output");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	prerequisites = {");
            sb.AppendLine(@"		giga_tech_amb_supertensiles_acot_$tier$");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	potential = {");
            sb.AppendLine(@"		nor = { ");
            sb.AppendLine(@"			has_global_flag = acot_building_forbidden");
            sb.AppendLine(@"			has_modifier = resort_colony");
            sb.AppendLine(@"			has_modifier = slave_colony ");
            sb.AppendLine(@"			has_modifier = crucible_colony");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		has_global_flag = @giga_amb_flag");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	allow = {");
            sb.AppendLine(@"		$allow$");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	# they're all upgrades, so this isn't necessary (and in fact breaks things)");
            sb.AppendLine(@"	# empire_limit = {");
            sb.AppendLine(@"	# 	base = 0");
            sb.AppendLine(@"	# 	modifier = {");
            sb.AppendLine(@"	# 		is_variable_set = giga_amb_cap");
            sb.AppendLine(@"	# 		add = giga_amb_cap");
            sb.AppendLine(@"	# 	}");
            sb.AppendLine(@"	# }");
            sb.AppendLine(@"");
            sb.AppendLine(@"	destroy_trigger = {");
            sb.AppendLine(@"		OR = {");
            sb.AppendLine(@"			has_modifier = resort_colony");
            sb.AppendLine(@"			has_modifier = slave_colony");
            sb.AppendLine(@"			has_modifier = crucible_colony");
            sb.AppendLine(@"			NOT = { has_global_flag = @giga_amb_flag }");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	upgrades = {");
            sb.AppendLine(@"		$upgrade$");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	country_modifier = {");
            sb.AppendLine(@"		country_resource_max_giga_sr_amb_megaconstruction_add = @giga_amb_hub_storage_$tier$");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	triggered_planet_modifier = {");
            sb.AppendLine(@"		potential = {");
            sb.AppendLine(@"			exists = owner");
            sb.AppendLine(@"			owner = {");
            sb.AppendLine(@"				is_gestalt = yes");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		modifier = {");
            sb.AppendLine(@"			job_giga_megaengineering_overseer_drone_acot_$tier$_add = 2");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	triggered_planet_modifier = {");
            sb.AppendLine(@"		potential = {");
            sb.AppendLine(@"			exists = owner");
            sb.AppendLine(@"			owner = {");
            sb.AppendLine(@"				is_gestalt = no");
            sb.AppendLine(@"				#is_fallen_empire_spiritualist = no");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		modifier = {");
            sb.AppendLine(@"			job_giga_megaengineering_overseer_acot_$tier$_add = 2");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	# triggered_planet_modifier = {");
            sb.AppendLine(@"	# 	potential = {");
            sb.AppendLine(@"	# 		exists = owner");
            sb.AppendLine(@"	# 		owner = {");
            sb.AppendLine(@"	# 			is_fallen_empire_spiritualist = yes");
            sb.AppendLine(@"	# 		}");
            sb.AppendLine(@"	# 	}");
            sb.AppendLine(@"	# 	modifier = {");
            sb.AppendLine(@"	# 		job_giga_amb_fe_celestial_architect_add = 2");
            sb.AppendLine(@"	# 	}");
            sb.AppendLine(@"	# }");
            sb.AppendLine(@"");
            sb.AppendLine(@"	triggered_desc = {");
            sb.AppendLine(@"		trigger = {");
            sb.AppendLine(@"			exists = owner");
            sb.AppendLine(@"			owner = {");
            sb.AppendLine(@"				is_gestalt = no");
            sb.AppendLine(@"				#is_fallen_empire_spiritualist = no");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		text = job_giga_megaengineering_overseer_acot_$tier$_effect_desc");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	triggered_desc = {");
            sb.AppendLine(@"		trigger = {");
            sb.AppendLine(@"			exists = owner");
            sb.AppendLine(@"			owner = {");
            sb.AppendLine(@"				is_gestalt = yes");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		text = job_giga_megaengineering_overseer_drone_acot_$tier$_effect_desc");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	# triggered_desc = {");
            sb.AppendLine(@"	# 	trigger = {");
            sb.AppendLine(@"	# 		exists = owner");
            sb.AppendLine(@"	# 		owner = {");
            sb.AppendLine(@"	# 			is_fallen_empire_spiritualist = yes");
            sb.AppendLine(@"	# 		}");
            sb.AppendLine(@"	# 	}");
            sb.AppendLine(@"	# 	text = job_giga_amb_fe_celestial_architect_effect_desc");
            sb.AppendLine(@"	# }");
            sb.AppendLine(@"}");


            var sb2 = new StringBuilder(257);
            sb2.AppendLine(@"inline_script = {");
            sb2.AppendLine(@"	script = ""buildings/building_giga_megaworkshop_acot""");
            sb2.AppendLine(@"	tier = ""delta""");
            sb2.AppendLine(@"");
            sb2.AppendLine(@"	allow = ""has_enigmatic_capital = yes""");
            sb2.AppendLine(@"	upgrade = ""building_giga_megaworkshop_acot_alpha""");
            sb2.AppendLine(@"");
            sb2.AppendLine(@"	resource1 = ""sr_dark_matter""");
            sb2.AppendLine(@"	resource2 = ""acot_sr_dark_energy""");
            sb2.AppendLine(@"}");

            var parser = new ParametrizedParser(new CodeParser(new Logger()));
            var result = parser.Process(sb.ToString(), sb2.ToString());
            result.Should().NotBeNullOrEmpty();
            var m = DIResolver.Get<IParserManager>();
            var parserResult = m.Parse(new ParserManagerArgs { File = "common\\buildings\\dummy.txt", GameType = "Stellaris", IsBinary = false, Lines = result.SplitOnNewLine() });
            parserResult.Count().Should().Be(1);
            parserResult.FirstOrDefault().Id.Should().Be("building_giga_megaworkshop_hub_acot_no_parameters");
        }

        /// <summary>
        /// Defines the test method GetObjectId_should_yield_results_using_parameters_with_extra_dolar_sign.
        /// </summary>
        [Fact]
        public void GetObjectId_should_yield_results_using_parameters_with_extra_dolar_sign()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder(3175);
            sb.AppendLine(@"building_giga_megaworkshop_hub_acot_$tier$$ = {");
            sb.AppendLine(@"	base_buildtime = @giga_amb_hub_time_$tier$");
            sb.AppendLine(@"	category = manufacturing");
            sb.AppendLine(@"	icon = ""acot_compat/building_giga_megaworkshop_hub_acot_$tier$""");
            sb.AppendLine(@"");
            sb.AppendLine(@"	can_build = no # upgrade");
            sb.AppendLine(@"");
            sb.AppendLine(@"	resources = {");
            sb.AppendLine(@"		category = planet_buildings");
            sb.AppendLine(@"		cost = {");
            sb.AppendLine(@"			minerals = @giga_amb_hub_cost_minerals_$tier$");
            sb.AppendLine(@"			$resource1$ = @giga_amb_hub_cost_resource1_$tier$");
            sb.AppendLine(@"			$resource2$ = @giga_amb_hub_cost_resource2_$tier$");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		upkeep = {");
            sb.AppendLine(@"			$resource1$ = @giga_amb_hub_upkeep_$tier$");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"");
            sb.AppendLine(@"		# auto production");
            sb.AppendLine(@"		upkeep = {");
            sb.AppendLine(@"			trigger = {");
            sb.AppendLine(@"				has_modifier = acot_modifier_ascended_city");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"");
            sb.AppendLine(@"			alloys = @giga_amb_megaengineer_$tier$_alloys_upkeep");
            sb.AppendLine(@"			unity = @giga_amb_megaengineer_$tier$_unity_upkeep");
            sb.AppendLine(@"			$resource1$ = @giga_amb_megaengineer_$tier$_resource1_upkeep");
            sb.AppendLine(@"			$resource2$ = @giga_amb_megaengineer_$tier$_resource2_upkeep");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		produces = {");
            sb.AppendLine(@"			trigger = {");
            sb.AppendLine(@"				has_modifier = acot_modifier_ascended_city");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"			giga_sr_amb_megaconstruction = @giga_amb_megaengineer_$tier$_output");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	prerequisites = {");
            sb.AppendLine(@"		giga_tech_amb_supertensiles_acot_$tier$");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	potential = {");
            sb.AppendLine(@"		nor = { ");
            sb.AppendLine(@"			has_global_flag = acot_building_forbidden");
            sb.AppendLine(@"			has_modifier = resort_colony");
            sb.AppendLine(@"			has_modifier = slave_colony ");
            sb.AppendLine(@"			has_modifier = crucible_colony");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		has_global_flag = @giga_amb_flag");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	allow = {");
            sb.AppendLine(@"		$allow$");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	# they're all upgrades, so this isn't necessary (and in fact breaks things)");
            sb.AppendLine(@"	# empire_limit = {");
            sb.AppendLine(@"	# 	base = 0");
            sb.AppendLine(@"	# 	modifier = {");
            sb.AppendLine(@"	# 		is_variable_set = giga_amb_cap");
            sb.AppendLine(@"	# 		add = giga_amb_cap");
            sb.AppendLine(@"	# 	}");
            sb.AppendLine(@"	# }");
            sb.AppendLine(@"");
            sb.AppendLine(@"	destroy_trigger = {");
            sb.AppendLine(@"		OR = {");
            sb.AppendLine(@"			has_modifier = resort_colony");
            sb.AppendLine(@"			has_modifier = slave_colony");
            sb.AppendLine(@"			has_modifier = crucible_colony");
            sb.AppendLine(@"			NOT = { has_global_flag = @giga_amb_flag }");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	upgrades = {");
            sb.AppendLine(@"		$upgrade$");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	country_modifier = {");
            sb.AppendLine(@"		country_resource_max_giga_sr_amb_megaconstruction_add = @giga_amb_hub_storage_$tier$");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	triggered_planet_modifier = {");
            sb.AppendLine(@"		potential = {");
            sb.AppendLine(@"			exists = owner");
            sb.AppendLine(@"			owner = {");
            sb.AppendLine(@"				is_gestalt = yes");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		modifier = {");
            sb.AppendLine(@"			job_giga_megaengineering_overseer_drone_acot_$tier$_add = 2");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	triggered_planet_modifier = {");
            sb.AppendLine(@"		potential = {");
            sb.AppendLine(@"			exists = owner");
            sb.AppendLine(@"			owner = {");
            sb.AppendLine(@"				is_gestalt = no");
            sb.AppendLine(@"				#is_fallen_empire_spiritualist = no");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		modifier = {");
            sb.AppendLine(@"			job_giga_megaengineering_overseer_acot_$tier$_add = 2");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	# triggered_planet_modifier = {");
            sb.AppendLine(@"	# 	potential = {");
            sb.AppendLine(@"	# 		exists = owner");
            sb.AppendLine(@"	# 		owner = {");
            sb.AppendLine(@"	# 			is_fallen_empire_spiritualist = yes");
            sb.AppendLine(@"	# 		}");
            sb.AppendLine(@"	# 	}");
            sb.AppendLine(@"	# 	modifier = {");
            sb.AppendLine(@"	# 		job_giga_amb_fe_celestial_architect_add = 2");
            sb.AppendLine(@"	# 	}");
            sb.AppendLine(@"	# }");
            sb.AppendLine(@"");
            sb.AppendLine(@"	triggered_desc = {");
            sb.AppendLine(@"		trigger = {");
            sb.AppendLine(@"			exists = owner");
            sb.AppendLine(@"			owner = {");
            sb.AppendLine(@"				is_gestalt = no");
            sb.AppendLine(@"				#is_fallen_empire_spiritualist = no");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		text = job_giga_megaengineering_overseer_acot_$tier$_effect_desc");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	triggered_desc = {");
            sb.AppendLine(@"		trigger = {");
            sb.AppendLine(@"			exists = owner");
            sb.AppendLine(@"			owner = {");
            sb.AppendLine(@"				is_gestalt = yes");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		text = job_giga_megaengineering_overseer_drone_acot_$tier$_effect_desc");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	# triggered_desc = {");
            sb.AppendLine(@"	# 	trigger = {");
            sb.AppendLine(@"	# 		exists = owner");
            sb.AppendLine(@"	# 		owner = {");
            sb.AppendLine(@"	# 			is_fallen_empire_spiritualist = yes");
            sb.AppendLine(@"	# 		}");
            sb.AppendLine(@"	# 	}");
            sb.AppendLine(@"	# 	text = job_giga_amb_fe_celestial_architect_effect_desc");
            sb.AppendLine(@"	# }");
            sb.AppendLine(@"}");


            var sb2 = new StringBuilder(257);
            sb2.AppendLine(@"inline_script = {");
            sb2.AppendLine(@"	script = ""buildings/building_giga_megaworkshop_acot""");
            sb2.AppendLine(@"	tier = ""delta""");
            sb2.AppendLine(@"");
            sb2.AppendLine(@"	allow = ""has_enigmatic_capital = yes""");
            sb2.AppendLine(@"	upgrade = ""building_giga_megaworkshop_acot_alpha""");
            sb2.AppendLine(@"");
            sb2.AppendLine(@"	resource1 = ""sr_dark_matter""");
            sb2.AppendLine(@"	resource2 = ""acot_sr_dark_energy""");
            sb2.AppendLine(@"}");

            var parser = new ParametrizedParser(new CodeParser(new Logger()));
            var result = parser.Process(sb.ToString(), sb2.ToString());

            result.Should().NotBeNullOrEmpty();
            var m = DIResolver.Get<IParserManager>();
            var parserResult = m.Parse(new ParserManagerArgs { File = "common\\buildings\\dummy.txt", GameType = "Stellaris", IsBinary = false, Lines = result.SplitOnNewLine() });
            parserResult.Count().Should().Be(1);
            parserResult.FirstOrDefault().Id.Should().Be("building_giga_megaworkshop_hub_acot_delta$");
        }

        /// <summary>
        /// Defines the test method GetObjectId_should_yield_results.
        /// </summary>
        [Fact]
        public void GetObjectId_should_yield_results_using_parameters_while_including_duplicates()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder(3175);
            sb.AppendLine(@"building_giga_megaworkshop_hub_acot_$tier$ = {");
            sb.AppendLine(@"	base_buildtime = @giga_amb_hub_time_$tier$");
            sb.AppendLine(@"	category = manufacturing");
            sb.AppendLine(@"	icon = ""acot_compat/building_giga_megaworkshop_hub_acot_$tier$""");
            sb.AppendLine(@"");
            sb.AppendLine(@"	can_build = no # upgrade");
            sb.AppendLine(@"");
            sb.AppendLine(@"	resources = {");
            sb.AppendLine(@"		category = planet_buildings");
            sb.AppendLine(@"		cost = {");
            sb.AppendLine(@"			minerals = @giga_amb_hub_cost_minerals_$tier$");
            sb.AppendLine(@"			$resource1$ = @giga_amb_hub_cost_resource1_$tier$");
            sb.AppendLine(@"			$resource2$ = @giga_amb_hub_cost_resource2_$tier$");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		upkeep = {");
            sb.AppendLine(@"			$resource1$ = @giga_amb_hub_upkeep_$tier$");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"");
            sb.AppendLine(@"		# auto production");
            sb.AppendLine(@"		upkeep = {");
            sb.AppendLine(@"			trigger = {");
            sb.AppendLine(@"				has_modifier = acot_modifier_ascended_city");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"");
            sb.AppendLine(@"			alloys = @giga_amb_megaengineer_$tier$_alloys_upkeep");
            sb.AppendLine(@"			unity = @giga_amb_megaengineer_$tier$_unity_upkeep");
            sb.AppendLine(@"			$resource1$ = @giga_amb_megaengineer_$tier$_resource1_upkeep");
            sb.AppendLine(@"			$resource2$ = @giga_amb_megaengineer_$tier$_resource2_upkeep");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		produces = {");
            sb.AppendLine(@"			trigger = {");
            sb.AppendLine(@"				has_modifier = acot_modifier_ascended_city");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"			giga_sr_amb_megaconstruction = @giga_amb_megaengineer_$tier$_output");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	prerequisites = {");
            sb.AppendLine(@"		giga_tech_amb_supertensiles_acot_$tier$");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	potential = {");
            sb.AppendLine(@"		nor = { ");
            sb.AppendLine(@"			has_global_flag = acot_building_forbidden");
            sb.AppendLine(@"			has_modifier = resort_colony");
            sb.AppendLine(@"			has_modifier = slave_colony ");
            sb.AppendLine(@"			has_modifier = crucible_colony");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		has_global_flag = @giga_amb_flag");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	allow = {");
            sb.AppendLine(@"		$allow$");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	# they're all upgrades, so this isn't necessary (and in fact breaks things)");
            sb.AppendLine(@"	# empire_limit = {");
            sb.AppendLine(@"	# 	base = 0");
            sb.AppendLine(@"	# 	modifier = {");
            sb.AppendLine(@"	# 		is_variable_set = giga_amb_cap");
            sb.AppendLine(@"	# 		add = giga_amb_cap");
            sb.AppendLine(@"	# 	}");
            sb.AppendLine(@"	# }");
            sb.AppendLine(@"");
            sb.AppendLine(@"	destroy_trigger = {");
            sb.AppendLine(@"		OR = {");
            sb.AppendLine(@"			has_modifier = resort_colony");
            sb.AppendLine(@"			has_modifier = slave_colony");
            sb.AppendLine(@"			has_modifier = crucible_colony");
            sb.AppendLine(@"			NOT = { has_global_flag = @giga_amb_flag }");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	upgrades = {");
            sb.AppendLine(@"		$upgrade$");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	country_modifier = {");
            sb.AppendLine(@"		country_resource_max_giga_sr_amb_megaconstruction_add = @giga_amb_hub_storage_$tier$");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	triggered_planet_modifier = {");
            sb.AppendLine(@"		potential = {");
            sb.AppendLine(@"			exists = owner");
            sb.AppendLine(@"			owner = {");
            sb.AppendLine(@"				is_gestalt = yes");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		modifier = {");
            sb.AppendLine(@"			job_giga_megaengineering_overseer_drone_acot_$tier$_add = 2");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	triggered_planet_modifier = {");
            sb.AppendLine(@"		potential = {");
            sb.AppendLine(@"			exists = owner");
            sb.AppendLine(@"			owner = {");
            sb.AppendLine(@"				is_gestalt = no");
            sb.AppendLine(@"				#is_fallen_empire_spiritualist = no");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		modifier = {");
            sb.AppendLine(@"			job_giga_megaengineering_overseer_acot_$tier$_add = 2");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	# triggered_planet_modifier = {");
            sb.AppendLine(@"	# 	potential = {");
            sb.AppendLine(@"	# 		exists = owner");
            sb.AppendLine(@"	# 		owner = {");
            sb.AppendLine(@"	# 			is_fallen_empire_spiritualist = yes");
            sb.AppendLine(@"	# 		}");
            sb.AppendLine(@"	# 	}");
            sb.AppendLine(@"	# 	modifier = {");
            sb.AppendLine(@"	# 		job_giga_amb_fe_celestial_architect_add = 2");
            sb.AppendLine(@"	# 	}");
            sb.AppendLine(@"	# }");
            sb.AppendLine(@"");
            sb.AppendLine(@"	triggered_desc = {");
            sb.AppendLine(@"		trigger = {");
            sb.AppendLine(@"			exists = owner");
            sb.AppendLine(@"			owner = {");
            sb.AppendLine(@"				is_gestalt = no");
            sb.AppendLine(@"				#is_fallen_empire_spiritualist = no");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		text = job_giga_megaengineering_overseer_acot_$tier$_effect_desc");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	triggered_desc = {");
            sb.AppendLine(@"		trigger = {");
            sb.AppendLine(@"			exists = owner");
            sb.AppendLine(@"			owner = {");
            sb.AppendLine(@"				is_gestalt = yes");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		text = job_giga_megaengineering_overseer_drone_acot_$tier$_effect_desc");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	# triggered_desc = {");
            sb.AppendLine(@"	# 	trigger = {");
            sb.AppendLine(@"	# 		exists = owner");
            sb.AppendLine(@"	# 		owner = {");
            sb.AppendLine(@"	# 			is_fallen_empire_spiritualist = yes");
            sb.AppendLine(@"	# 		}");
            sb.AppendLine(@"	# 	}");
            sb.AppendLine(@"	# 	text = job_giga_amb_fe_celestial_architect_effect_desc");
            sb.AppendLine(@"	# }");
            sb.AppendLine(@"}");
            sb.AppendLine(@"building_giga_megaworkshop_hub_acot_$tier$ = {");
            sb.AppendLine(@"}");


            var sb2 = new StringBuilder(257);
            sb2.AppendLine(@"inline_script = {");
            sb2.AppendLine(@"	script = ""buildings/building_giga_megaworkshop_acot""");
            sb2.AppendLine(@"	tier = ""delta""");
            sb2.AppendLine(@"");
            sb2.AppendLine(@"	allow = ""has_enigmatic_capital = yes""");
            sb2.AppendLine(@"	upgrade = ""building_giga_megaworkshop_acot_alpha""");
            sb2.AppendLine(@"");
            sb2.AppendLine(@"	resource1 = ""sr_dark_matter""");
            sb2.AppendLine(@"	resource2 = ""acot_sr_dark_energy""");
            sb2.AppendLine(@"}");

            var parser = new ParametrizedParser(new CodeParser(new Logger()));
            var result = parser.Process(sb.ToString(), sb2.ToString());

            result.Should().NotBeNullOrEmpty();
            var m = DIResolver.Get<IParserManager>();
            var parserResult = m.Parse(new ParserManagerArgs { File = "common\\buildings\\dummy.txt", GameType = "Stellaris", IsBinary = false, Lines = result.SplitOnNewLine() });
            parserResult.Count().Should().Be(2);
            parserResult.All(p => p.Id == "building_giga_megaworkshop_hub_acot_delta").Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method GetObjectId_should_yield_multiple_results_using_parameters.
        /// </summary>
        [Fact]
        public void GetObjectId_should_yield_multiple_results_using_parameters()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder(3175);
            sb.AppendLine(@"building_giga_megaworkshop_hub_acot_$tier$ = {");
            sb.AppendLine(@"	base_buildtime = @giga_amb_hub_time_$tier$");
            sb.AppendLine(@"	category = manufacturing");
            sb.AppendLine(@"	icon = ""acot_compat/building_giga_megaworkshop_hub_acot_$tier$""");
            sb.AppendLine(@"");
            sb.AppendLine(@"	can_build = no # upgrade");
            sb.AppendLine(@"");
            sb.AppendLine(@"	resources = {");
            sb.AppendLine(@"		category = planet_buildings");
            sb.AppendLine(@"		cost = {");
            sb.AppendLine(@"			minerals = @giga_amb_hub_cost_minerals_$tier$");
            sb.AppendLine(@"			$resource1$ = @giga_amb_hub_cost_resource1_$tier$");
            sb.AppendLine(@"			$resource2$ = @giga_amb_hub_cost_resource2_$tier$");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		upkeep = {");
            sb.AppendLine(@"			$resource1$ = @giga_amb_hub_upkeep_$tier$");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"");
            sb.AppendLine(@"		# auto production");
            sb.AppendLine(@"		upkeep = {");
            sb.AppendLine(@"			trigger = {");
            sb.AppendLine(@"				has_modifier = acot_modifier_ascended_city");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"");
            sb.AppendLine(@"			alloys = @giga_amb_megaengineer_$tier$_alloys_upkeep");
            sb.AppendLine(@"			unity = @giga_amb_megaengineer_$tier$_unity_upkeep");
            sb.AppendLine(@"			$resource1$ = @giga_amb_megaengineer_$tier$_resource1_upkeep");
            sb.AppendLine(@"			$resource2$ = @giga_amb_megaengineer_$tier$_resource2_upkeep");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		produces = {");
            sb.AppendLine(@"			trigger = {");
            sb.AppendLine(@"				has_modifier = acot_modifier_ascended_city");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"			giga_sr_amb_megaconstruction = @giga_amb_megaengineer_$tier$_output");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	prerequisites = {");
            sb.AppendLine(@"		giga_tech_amb_supertensiles_acot_$tier$");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	potential = {");
            sb.AppendLine(@"		nor = { ");
            sb.AppendLine(@"			has_global_flag = acot_building_forbidden");
            sb.AppendLine(@"			has_modifier = resort_colony");
            sb.AppendLine(@"			has_modifier = slave_colony ");
            sb.AppendLine(@"			has_modifier = crucible_colony");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		has_global_flag = @giga_amb_flag");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	allow = {");
            sb.AppendLine(@"		$allow$");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	# they're all upgrades, so this isn't necessary (and in fact breaks things)");
            sb.AppendLine(@"	# empire_limit = {");
            sb.AppendLine(@"	# 	base = 0");
            sb.AppendLine(@"	# 	modifier = {");
            sb.AppendLine(@"	# 		is_variable_set = giga_amb_cap");
            sb.AppendLine(@"	# 		add = giga_amb_cap");
            sb.AppendLine(@"	# 	}");
            sb.AppendLine(@"	# }");
            sb.AppendLine(@"");
            sb.AppendLine(@"	destroy_trigger = {");
            sb.AppendLine(@"		OR = {");
            sb.AppendLine(@"			has_modifier = resort_colony");
            sb.AppendLine(@"			has_modifier = slave_colony");
            sb.AppendLine(@"			has_modifier = crucible_colony");
            sb.AppendLine(@"			NOT = { has_global_flag = @giga_amb_flag }");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	upgrades = {");
            sb.AppendLine(@"		$upgrade$");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	country_modifier = {");
            sb.AppendLine(@"		country_resource_max_giga_sr_amb_megaconstruction_add = @giga_amb_hub_storage_$tier$");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	triggered_planet_modifier = {");
            sb.AppendLine(@"		potential = {");
            sb.AppendLine(@"			exists = owner");
            sb.AppendLine(@"			owner = {");
            sb.AppendLine(@"				is_gestalt = yes");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		modifier = {");
            sb.AppendLine(@"			job_giga_megaengineering_overseer_drone_acot_$tier$_add = 2");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	triggered_planet_modifier = {");
            sb.AppendLine(@"		potential = {");
            sb.AppendLine(@"			exists = owner");
            sb.AppendLine(@"			owner = {");
            sb.AppendLine(@"				is_gestalt = no");
            sb.AppendLine(@"				#is_fallen_empire_spiritualist = no");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		modifier = {");
            sb.AppendLine(@"			job_giga_megaengineering_overseer_acot_$tier$_add = 2");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	# triggered_planet_modifier = {");
            sb.AppendLine(@"	# 	potential = {");
            sb.AppendLine(@"	# 		exists = owner");
            sb.AppendLine(@"	# 		owner = {");
            sb.AppendLine(@"	# 			is_fallen_empire_spiritualist = yes");
            sb.AppendLine(@"	# 		}");
            sb.AppendLine(@"	# 	}");
            sb.AppendLine(@"	# 	modifier = {");
            sb.AppendLine(@"	# 		job_giga_amb_fe_celestial_architect_add = 2");
            sb.AppendLine(@"	# 	}");
            sb.AppendLine(@"	# }");
            sb.AppendLine(@"");
            sb.AppendLine(@"	triggered_desc = {");
            sb.AppendLine(@"		trigger = {");
            sb.AppendLine(@"			exists = owner");
            sb.AppendLine(@"			owner = {");
            sb.AppendLine(@"				is_gestalt = no");
            sb.AppendLine(@"				#is_fallen_empire_spiritualist = no");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		text = job_giga_megaengineering_overseer_acot_$tier$_effect_desc");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	triggered_desc = {");
            sb.AppendLine(@"		trigger = {");
            sb.AppendLine(@"			exists = owner");
            sb.AppendLine(@"			owner = {");
            sb.AppendLine(@"				is_gestalt = yes");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		text = job_giga_megaengineering_overseer_drone_acot_$tier$_effect_desc");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	# triggered_desc = {");
            sb.AppendLine(@"	# 	trigger = {");
            sb.AppendLine(@"	# 		exists = owner");
            sb.AppendLine(@"	# 		owner = {");
            sb.AppendLine(@"	# 			is_fallen_empire_spiritualist = yes");
            sb.AppendLine(@"	# 		}");
            sb.AppendLine(@"	# 	}");
            sb.AppendLine(@"	# 	text = job_giga_amb_fe_celestial_architect_effect_desc");
            sb.AppendLine(@"	# }");
            sb.AppendLine(@"}");
            sb.AppendLine(@"building_giga_megaworkshop_hub_acot_2_$tier$ = {");
            sb.AppendLine(@"}");


            var sb2 = new StringBuilder(257);
            sb2.AppendLine(@"inline_script = {");
            sb2.AppendLine(@"	script = ""buildings/building_giga_megaworkshop_acot""");
            sb2.AppendLine(@"	tier = ""delta""");
            sb2.AppendLine(@"");
            sb2.AppendLine(@"	allow = ""has_enigmatic_capital = yes""");
            sb2.AppendLine(@"	upgrade = ""building_giga_megaworkshop_acot_alpha""");
            sb2.AppendLine(@"");
            sb2.AppendLine(@"	resource1 = ""sr_dark_matter""");
            sb2.AppendLine(@"	resource2 = ""acot_sr_dark_energy""");
            sb2.AppendLine(@"}");

            var parser = new ParametrizedParser(new CodeParser(new Logger()));
            var result = parser.Process(sb.ToString(), sb2.ToString());

            result.Should().NotBeNullOrEmpty();
            var m = DIResolver.Get<IParserManager>();
            var parserResult = m.Parse(new ParserManagerArgs { File = "common\\buildings\\dummy.txt", GameType = "Stellaris", IsBinary = false, Lines = result.SplitOnNewLine() });
            parserResult.Count().Should().Be(2);
            parserResult.Any(p => p.Id == "building_giga_megaworkshop_hub_acot_delta").Should().BeTrue();
            parserResult.Any(p => p.Id == "building_giga_megaworkshop_hub_acot_2_delta").Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method GetObjectId_on_first_levelshould_yield_results.
        /// </summary>
        [Fact]
        public void GetObjectId_on_first_level_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder(700);
            sb.AppendLine(@"key = ""BIO_PROPULSION_$LEVEL$_$CORRESPONDING_SIZE$""");
            sb.AppendLine(@"size = small");
            sb.AppendLine(@"icon = ""GFX_ship_part_bio_thruster_$LEVEL$""");
            sb.AppendLine(@"icon_frame = 1");
            sb.AppendLine(@"power = 0");
            sb.AppendLine(@"");
            sb.AppendLine(@"resources = {");
            sb.AppendLine(@"	category = ship_components");
            sb.AppendLine(@"	inline_script = {");
            sb.AppendLine(@"		script = ""grand_archive/mutations/component_dynamic_cost""");
            sb.AppendLine(@"		COST = $COST$");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"	cost = {");
            sb.AppendLine(@"		sr_dark_matter	= $DARK_MATTER$");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"}");
            sb.AppendLine(@"");
            sb.AppendLine(@"modifier = {");
            sb.AppendLine(@"	ship_base_speed_mult = $SPEED$");
            sb.AppendLine(@"	ship_evasion_add = $EVASION$");
            sb.AppendLine(@"}");
            sb.AppendLine(@"");
            sb.AppendLine(@"prerequisites = { $PREREQUISITE$ }");
            sb.AppendLine(@"component_set = ""thruster_components_bio""");
            sb.AppendLine(@"inline_script = {");
            sb.AppendLine(@"	script = grand_archive/mutations/core_components/upgrade_thrusters_bio_$LEVEL$");
            sb.AppendLine(@"	CORRESPONDING_SIZE = $CORRESPONDING_SIZE$");
            sb.AppendLine(@"}");
            sb.AppendLine(@"");
            sb.AppendLine(@"size_restriction = { $SIZE_RESTRICTION$ }");
            sb.AppendLine(@"");
            sb.AppendLine(@"ai_weight = {");
            sb.AppendLine(@"	weight = $LEVEL$");
            sb.AppendLine(@"}");


            var sb2 = new StringBuilder(374);
            sb2.AppendLine(@"utility_component_template = {");
            sb2.AppendLine(@"	inline_script = {");
            sb2.AppendLine(@"		script = grand_archive/mutations/core_components/component_thrusters_bio");
            sb2.AppendLine(@"		LEVEL = 5");
            sb2.AppendLine(@"		CORRESPONDING_SIZE = BATTLESHIP");
            sb2.AppendLine(@"		PREREQUISITE = ""tech_dark_matter_propulsion tech_thrusters_bio_integration""");
            sb2.AppendLine(@"		COST = 384");
            sb2.AppendLine(@"		DARK_MATTER = 4");
            sb2.AppendLine(@"		SPEED = 1");
            sb2.AppendLine(@"		EVASION = 8");
            sb2.AppendLine(@"		SIZE_RESTRICTION = ""space_whale_5 voidworms_large cutholoids""");
            sb2.AppendLine(@"	}");
            sb2.AppendLine(@"}");

            var parser = new ParametrizedParser(new CodeParser(new Logger()));
            var result = parser.Process(sb.ToString(), sb2.ToString());
            result.Should().NotBeNullOrEmpty();
            var m = DIResolver.Get<IParserManager>();
            var parserResult = m.Parse(new ParserManagerArgs { File = "common\\component_templates\\dummy.txt", GameType = "Stellaris", IsBinary = false, Lines = result.SplitOnNewLine() });
            parserResult.Count().Should().Be(1);
            parserResult.Any(p => p.Id.Equals("BIO_PROPULSION_5_BATTLESHIP")).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method GetObjectId_on_first_level_should_yield_results_with_simple_inline.
        /// </summary>
        [Fact]
        public void GetObjectId_on_first_level_should_yield_results_with_simple_inline()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder(328);
            sb.AppendLine(@"# ship class placeholder");
            sb.AppendLine(@"entity = corvette_entity");
            sb.AppendLine(@"resources = { category = starbase_stations }");
            sb.AppendLine(@"potential_construction = { always = no }");
            sb.AppendLine(@"possible_construction = { always = no }");
            sb.AppendLine(@"is_designable = no");
            sb.AppendLine(@"enable_default_design = yes");
            sb.AppendLine(@"prerequisites = { }");
            sb.AppendLine(@"class = shipclass_starbase");
            sb.AppendLine(@"icon_frame = 1");
            sb.AppendLine(@"icon = ship_size_military_station");

            var sb2 = new StringBuilder(77);
            sb2.AppendLine(@"rs_heavy_dreadnought = {");
            sb2.AppendLine(@"    inline_script = giga_placeholders/ship_sizes");
            sb2.AppendLine(@"}");

            var parser = new ParametrizedParser(new CodeParser(new Logger()));
            var result = parser.Process(sb.ToString(), sb2.ToString());
            result.Should().NotBeNullOrEmpty();
            var m = DIResolver.Get<IParserManager>();
            var parserResult = m.Parse(new ParserManagerArgs { File = "common\\ship_sizes\\dummy.txt", GameType = "Stellaris", IsBinary = false, Lines = result.SplitOnNewLine() });
            parserResult.Count().Should().Be(1);
            parserResult.Any(p => p.Id.Equals("rs_heavy_dreadnought")).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method GetObjectId_should_yield_results_with_simple_inline.
        /// </summary>
        [Fact]
        public void GetObjectId_should_yield_results_with_simple_inline()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder();
            sb.AppendLine(@"rs_heavy_dreadnought = {");
            sb.AppendLine(@"# ship class placeholder");
            sb.AppendLine(@"entity = corvette_entity");
            sb.AppendLine(@"resources = { category = starbase_stations }");
            sb.AppendLine(@"potential_construction = { always = no }");
            sb.AppendLine(@"possible_construction = { always = no }");
            sb.AppendLine(@"is_designable = no");
            sb.AppendLine(@"enable_default_design = yes");
            sb.AppendLine(@"prerequisites = { }");
            sb.AppendLine(@"class = shipclass_starbase");
            sb.AppendLine(@"icon_frame = 1");
            sb.AppendLine(@"icon = ship_size_military_station");
            sb.AppendLine(@"}");


            var sb2 = new StringBuilder(77);
            sb2.AppendLine(@"");
            sb2.AppendLine(@"inline_script = giga_placeholders/ship_sizes");
            sb2.AppendLine(@"");

            var parser = new ParametrizedParser(new CodeParser(new Logger()));
            var result = parser.Process(sb.ToString(), sb2.ToString());
            result.Should().NotBeNullOrEmpty();
            var m = DIResolver.Get<IParserManager>();
            var parserResult = m.Parse(new ParserManagerArgs { File = "common\\ship_sizes\\dummy.txt", GameType = "Stellaris", IsBinary = false, Lines = result.SplitOnNewLine() });
            parserResult.Count().Should().Be(1);
            parserResult.Any(p => p.Id.Equals("rs_heavy_dreadnought")).Should().BeTrue();
        }


        /// <summary>
        /// Defines the test method GetObjectId_should_understand_math_expressions.
        /// </summary>
        [Fact]
        public void GetObjectId_should_understand_math_expressions()
        {
            DISetup.SetupContainer();
            CurrentLocale.SetCurrent("en");

            var sb = new StringBuilder();
            sb.AppendLine(@"inline_script = {");
            sb.AppendLine(@"	script = merger_of_rules/parts/switch");
            sb.AppendLine(@"	file = merger_of_rules/parts/toggled_code_case_");
            sb.AppendLine(@"	");
            sb.AppendLine(@"	value = @[ (-1 * ((-1 * (($toggle$*$toggle$) / (($toggle$*$toggle$)+1))) - ((((-1 * (($toggle$*$toggle$) / (($toggle$*$toggle$)+1))) % 1) + 1) % 1))) ]");
            sb.AppendLine(@"");
            sb.AppendLine(@"	params = ""code = \""$code$\"""" ");
            sb.AppendLine(@"}");

            var sb2 = new StringBuilder(77);
            sb2.AppendLine(@"	inline_script = {");
            sb2.AppendLine(@"		script = merger_of_rules/toggled_code");
            sb2.AppendLine(@"		toggle = 1");
            sb2.AppendLine(@"	}");

            var parser = new ParametrizedParser(new CodeParser(new Logger()));
            var result = parser.Process(sb.ToString(), sb2.ToString());
            result.Should().NotBeNullOrEmpty();
            var m = DIResolver.Get<IParserManager>();
            var parserResult = m.Parse(new ParserManagerArgs { File = "common\\ship_sizes\\dummy.txt", GameType = "Stellaris", IsBinary = false, Lines = result.SplitOnNewLine() });
            parserResult.Count().Should().Be(1);
            parserResult.FirstOrDefault()!.Code.Should().Be("inline_script = {\r\n    script = merger_of_rules/parts/switch\r\n    file = merger_of_rules/parts/toggled_code_case_\r\n    value = 1\r\n    params = code = \\\"$code$\\\r\n}");
        }

        /// <summary>
        /// Defines the test method GetScriptPath_should_yield_results.
        /// </summary>
        [Fact]
        public void GetScriptPath_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder(257);
            sb.AppendLine(@"inline_script = {");
            sb.AppendLine(@"	script = ""buildings/building_giga_megaworkshop_acot""");
            sb.AppendLine(@"	tier = ""delta""");
            sb.AppendLine(@"");
            sb.AppendLine(@"	allow = ""has_enigmatic_capital = yes""");
            sb.AppendLine(@"	upgrade = ""building_giga_megaworkshop_acot_alpha""");
            sb.AppendLine(@"");
            sb.AppendLine(@"	resource1 = ""sr_dark_matter""");
            sb.AppendLine(@"	resource2 = ""acot_sr_dark_energy""");
            sb.AppendLine(@"}");

            var parser = new ParametrizedParser(new CodeParser(new Logger()));
            var result = parser.GetScriptPath(sb.ToString());
            result.Should().Be("buildings\\building_giga_megaworkshop_acot");
        }

        /// <summary>
        /// Defines the test method GetScriptPath_should_not_yield_results.
        /// </summary>
        [Fact]
        public void GetScriptPath_should_not_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder(257);
            sb.AppendLine(@"inline_script = {");
            sb.AppendLine(@"	tier = ""delta""");
            sb.AppendLine(@"");
            sb.AppendLine(@"	allow = ""has_enigmatic_capital = yes""");
            sb.AppendLine(@"	upgrade = ""building_giga_megaworkshop_acot_alpha""");
            sb.AppendLine(@"");
            sb.AppendLine(@"	resource1 = ""sr_dark_matter""");
            sb.AppendLine(@"	resource2 = ""acot_sr_dark_energy""");
            sb.AppendLine(@"}");

            var parser = new ParametrizedParser(new CodeParser(new Logger()));
            var result = parser.GetScriptPath(sb.ToString());
            result.Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Defines the test method GetScriptPath_as_sub_element_should_yield_results.
        /// </summary>
        [Fact]
        public void GetScriptPath_as_sub_element_should_yield_results()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder(374);
            sb.AppendLine(@"utility_component_template = {");
            sb.AppendLine(@"	inline_script = {");
            sb.AppendLine(@"		script = grand_archive/mutations/core_components/component_thrusters_bio");
            sb.AppendLine(@"		LEVEL = 5");
            sb.AppendLine(@"		CORRESPONDING_SIZE = BATTLESHIP");
            sb.AppendLine(@"		PREREQUISITE = ""tech_dark_matter_propulsion tech_thrusters_bio_integration""");
            sb.AppendLine(@"		COST = 384");
            sb.AppendLine(@"		DARK_MATTER = 4");
            sb.AppendLine(@"		SPEED = 1");
            sb.AppendLine(@"		EVASION = 8");
            sb.AppendLine(@"		SIZE_RESTRICTION = ""space_whale_5 voidworms_large cutholoids""");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"}");

            var parser = new ParametrizedParser(new CodeParser(new Logger()));
            var result = parser.GetScriptPath(sb.ToString());
            result.Should().Be("grand_archive\\mutations\\core_components\\component_thrusters_bio");
        }

        /// <summary>
        /// Defines the test method GetScriptPath_as_sub_element_should_handle_simple_inline_scripts.
        /// </summary>
        [Fact]
        public void GetScriptPath_as_sub_element_should_handle_simple_inline_scripts()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder(77);
            sb.AppendLine(@"rs_heavy_dreadnought = {");
            sb.AppendLine(@"    inline_script = giga_placeholders/ship_sizes");
            sb.AppendLine(@"}");

            var parser = new ParametrizedParser(new CodeParser(new Logger()));
            var result = parser.GetScriptPath(sb.ToString());
            result.Should().Be("giga_placeholders\\ship_sizes");
        }

        /// <summary>
        /// Defines the test method GetScriptPath_as_should_handle_simple_inline_scripts.
        /// </summary>
        [Fact]
        public void GetScriptPath_as_should_handle_simple_inline_scripts()
        {
            DISetup.SetupContainer();

            var sb = new StringBuilder(77);
            sb.AppendLine(@"");
            sb.AppendLine(@"inline_script = giga_placeholders/ship_sizes");
            sb.AppendLine(@"");

            var parser = new ParametrizedParser(new CodeParser(new Logger()));
            var result = parser.GetScriptPath(sb.ToString());
            result.Should().Be("giga_placeholders\\ship_sizes");
        }
    }
}
