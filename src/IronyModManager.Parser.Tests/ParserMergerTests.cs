// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 10-19-2024
//
// Last Modified By : Mario
// Last Modified On : 10-19-2024
// ***********************************************************************
// <copyright file="ParserMergerTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AwesomeAssertions;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Shared;
using IronyModManager.Tests.Common;
using Xunit;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class ParserMergerTests.
    /// </summary>
    public class ParserMergerTests
    {
        /// <summary>
        /// Defines the test method CanParse_should_be_false.
        /// </summary>
        [Fact]
        public void CanParse_should_be_false()
        {
            var parser = new ParserMerger(new CodeParser(new Logger()), new Logger());
            parser.CanParse(new CanParseArgs()).Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Parse_should_throw_exception.
        /// </summary>
        [Fact]
        public void Parse_should_throw_exception()
        {
            var parser = new ParserMerger(new CodeParser(new Logger()), new Logger());
            Exception res;
            try
            {
                parser.Parse(new ParserArgs());
                res = null;
            }
            catch (NotImplementedException ex)
            {
                res = ex;
            }

            res.Should().NotBeNull();
        }

        /// <summary>
        /// Defines the test method Should_merge_code.
        /// </summary>
        [Fact]
        public void Should_merge_code()
        {
            DISetup.SetupContainer();

            var parser = new ParserMerger(new CodeParser(new Logger()), new Logger());
            var sb = new StringBuilder(3101);
            sb.AppendLine(@"auth_corporate = {");
            sb.AppendLine(@"	election_term_years = 20");
            sb.AppendLine(@"	election_type = oligarchic");
            sb.AppendLine(@"	can_have_emergency_elections = yes");
            sb.AppendLine(@"	max_election_candidates = 4");
            sb.AppendLine(@"	localization_postfix = corporate");
            sb.AppendLine(@"");
            sb.AppendLine(@"	has_agendas = yes");
            sb.AppendLine(@"");
            sb.AppendLine(@"	ruler_council_position = councilor_ruler_corporate");
            sb.AppendLine(@"");
            sb.AppendLine(@"	potential = {");
            sb.AppendLine(@"		country_type = { NOT = { value = primitive } }");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	playable = {");
            sb.AppendLine(@"		has_megacorp = yes");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"	possible = {");
            sb.AppendLine(@"		origin = {");
            sb.AppendLine(@"			NOR = {");
            sb.AppendLine(@"				text = origin_legendary_leader_no_gov_change");
            sb.AppendLine(@"				value = origin_legendary_leader_dictatorial");
            sb.AppendLine(@"				value = origin_legendary_leader_imperial");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		ethics = {");
            sb.AppendLine(@"			NOR = {");
            sb.AppendLine(@"				value = ethic_gestalt_consciousness");
            sb.AppendLine(@"				value = ethic_fanatic_egalitarian");
            sb.AppendLine(@"				value = ethic_fanatic_authoritarian");
            sb.AppendLine(@"			}");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	country_modifier = {");
            sb.AppendLine(@"		commercial_pact_mult = 0.20");
            sb.AppendLine(@"		empire_size_colonies_mult = 0.50");
            sb.AppendLine(@"		external_leader_pool_add = 1");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	advanced_authority_swap	= {");
            sb.AppendLine(@"		name = ""auth_cyber_creed_corporate""");
            sb.AppendLine(@"		description = ""auth_cyber_creed_corporate_desc""");
            sb.AppendLine(@"		inherit_icon = no");
            sb.AppendLine(@"		inherit_effects = no");
            sb.AppendLine(@"		trigger = {");
            sb.AppendLine(@"			is_scope_valid = yes");
            sb.AppendLine(@"			has_country_flag = cyber_creed_advanced_government");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		modifier = {");
            sb.AppendLine(@"			pop_ethic_spiritualist_attraction_mult = 0.2");
            sb.AppendLine(@"			pop_cyborg_happiness = 0.15");
            sb.AppendLine(@"			pop_non_cyborg_happiness = -0.15");
            sb.AppendLine(@"			pop_factions_produces_mult = 0.1");
            sb.AppendLine(@"			all_technology_research_speed = 0.05");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	advanced_authority_swap	= {");
            sb.AppendLine(@"		name = ""auth_cyber_corporate_individualist""");
            sb.AppendLine(@"		description = ""auth_cyber_corporate_individualist_desc""");
            sb.AppendLine(@"		inherit_icon = no");
            sb.AppendLine(@"		inherit_effects = no");
            sb.AppendLine(@"		trigger = {");
            sb.AppendLine(@"			is_scope_valid = yes");
            sb.AppendLine(@"			has_country_flag = cyber_individualist");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		modifier = {");
            sb.AppendLine(@"			custom_tooltip = auth_cyber_corporate_individualist_tt");
            sb.AppendLine(@"			starbase_trade_protection_add = 15");
            sb.AppendLine(@"			planet_crime_add = 20");
            sb.AppendLine(@"			pop_lifestyle_trade_value_add = 1");
            sb.AppendLine(@"			pop_cyborg_happiness = 0.1");
            sb.AppendLine(@"			show_only_custom_tooltip = no");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	advanced_authority_swap	= {");
            sb.AppendLine(@"		name = ""auth_cyber_corporate_collectivist""");
            sb.AppendLine(@"		description = ""auth_cyber_corporate_collectivist_desc""");
            sb.AppendLine(@"		inherit_icon = no");
            sb.AppendLine(@"		inherit_effects = no");
            sb.AppendLine(@"		trigger = {");
            sb.AppendLine(@"			is_scope_valid = yes");
            sb.AppendLine(@"			has_country_flag = cyber_collectivist");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		modifier = {");
            sb.AppendLine(@"			custom_tooltip = auth_cyber_corporate_collectivist_tt");
            sb.AppendLine(@"			starbase_trade_protection_add = 15");
            sb.AppendLine(@"			planet_crime_add = 20");
            sb.AppendLine(@"			council_agenda_progress_speed = 0.25");
            sb.AppendLine(@"			show_only_custom_tooltip = no");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	advanced_authority_swap	= {");
            sb.AppendLine(@"		name = ""auth_synth_corporate_physical""");
            sb.AppendLine(@"		description = ""auth_synth_corporate_physical_desc""");
            sb.AppendLine(@"		inherit_icon = no");
            sb.AppendLine(@"		inherit_effects = no");
            sb.AppendLine(@"		trigger = {");
            sb.AppendLine(@"			is_scope_valid = yes");
            sb.AppendLine(@"			has_country_flag = synth_physical");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		modifier = {");
            sb.AppendLine(@"			custom_tooltip = auth_synth_corporate_physical_tt");
            sb.AppendLine(@"			trade_value_mult = 0.05");
            sb.AppendLine(@"			show_only_custom_tooltip = no");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	advanced_authority_swap	= {");
            sb.AppendLine(@"		name = ""auth_synth_corporate_virtual""");
            sb.AppendLine(@"		description = ""auth_synth_corporate_virtual_desc""");
            sb.AppendLine(@"		inherit_icon = no");
            sb.AppendLine(@"		inherit_effects = no");
            sb.AppendLine(@"		trigger = {");
            sb.AppendLine(@"			is_scope_valid = yes");
            sb.AppendLine(@"			has_country_flag = synth_virtual");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"		modifier = {");
            sb.AppendLine(@"			custom_tooltip = auth_synth_corporate_virtual_tt");
            sb.AppendLine(@"			intel_decryption_add = 1");
            sb.AppendLine(@"			show_only_custom_tooltip = no");
            sb.AppendLine(@"		}");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"");
            sb.AppendLine(@"	tags = {");
            sb.AppendLine(@"		AUTHORITY_SUCCESSION_TYPE");
            sb.AppendLine(@"		AUTHORITY_OLIGARCHIC_SUCCESSION");
            sb.AppendLine(@"	}");
            sb.AppendLine(@"}");

            var sb2 = new StringBuilder(489);
            sb2.AppendLine(@"auth_corporate = {");
            sb2.AppendLine(@"	advanced_authority_swap	= {");
            sb2.AppendLine(@"		name = ""auth_cyber_creed_democratic""");
            sb2.AppendLine(@"		description = ""auth_cyber_creed_democratic_desc""");
            sb2.AppendLine(@"		inherit_icon = no");
            sb2.AppendLine(@"		inherit_effects = no");
            sb2.AppendLine(@"		trigger = {");
            sb2.AppendLine(@"			is_scope_valid = yes");
            sb2.AppendLine(@"			#has_country_flag = cyber_creed_advanced_government");
            sb2.AppendLine(@"		}");
            sb2.AppendLine(@"		modifier = {");
            sb2.AppendLine(@"			pop_ethic_spiritualist_attraction_mult = 0.24");
            sb2.AppendLine(@"			pop_cyborg_happiness = 0.15");
            sb2.AppendLine(@"			pop_non_cyborg_happiness = -0.15");
            sb2.AppendLine(@"			pop_factions_produces_mult = 0.10");
            sb2.AppendLine(@"			all_technology_research_speed = 0.05");
            sb2.AppendLine(@"		}");
            sb2.AppendLine(@"	}");
            sb.AppendLine(@"}");

            var sb3 = new StringBuilder(491);
            sb3.AppendLine(@"auth_corporate = {");
            sb3.AppendLine(@"	advanced_authority_swap	= {");
            sb3.AppendLine(@"		name = ""auth_cyber_creed_democratic2""");
            sb3.AppendLine(@"		description = ""auth_cyber_creed_democratic_desc2""");
            sb3.AppendLine(@"		inherit_icon = no");
            sb3.AppendLine(@"		inherit_effects = no");
            sb3.AppendLine(@"		trigger = {");
            sb3.AppendLine(@"			is_scope_valid = yes");
            sb3.AppendLine(@"			#has_country_flag = cyber_creed_advanced_government");
            sb3.AppendLine(@"		}");
            sb3.AppendLine(@"		modifier = {");
            sb3.AppendLine(@"			pop_ethic_spiritualist_attraction_mult = 0.3");
            sb3.AppendLine(@"			pop_cyborg_happiness = 0.15");
            sb3.AppendLine(@"			pop_non_cyborg_happiness = -0.15");
            sb3.AppendLine(@"			pop_factions_produces_mult = 0.10");
            sb3.AppendLine(@"			all_technology_research_speed = 0.05");
            sb3.AppendLine(@"		}");
            sb3.AppendLine(@"	}	");
            sb.AppendLine(@"}");

            var code = parser.MergeTopLevel(sb.ToString().SplitOnNewLine(), "test.txt", "advanced_authority_swap", sb2.ToString().SplitOnNewLine(), false);

            var sbResult1 = new StringBuilder();
            sbResult1.AppendLine(@"auth_corporate = {");
            sbResult1.AppendLine(@"    election_term_years = 20");
            sbResult1.AppendLine(@"    election_type = oligarchic");
            sbResult1.AppendLine(@"    can_have_emergency_elections = yes");
            sbResult1.AppendLine(@"    max_election_candidates = 4");
            sbResult1.AppendLine(@"    localization_postfix = corporate");
            sbResult1.AppendLine(@"    has_agendas = yes");
            sbResult1.AppendLine(@"    ruler_council_position = councilor_ruler_corporate");
            sbResult1.AppendLine(@"    potential = {");
            sbResult1.AppendLine(@"        country_type = {");
            sbResult1.AppendLine(@"            NOT = {");
            sbResult1.AppendLine(@"                value = primitive");
            sbResult1.AppendLine(@"            }");
            sbResult1.AppendLine(@"        }");
            sbResult1.AppendLine(@"    }");
            sbResult1.AppendLine(@"    playable = {");
            sbResult1.AppendLine(@"        has_megacorp = yes");
            sbResult1.AppendLine(@"    }");
            sbResult1.AppendLine(@"    possible = {");
            sbResult1.AppendLine(@"        origin = {");
            sbResult1.AppendLine(@"            NOR = {");
            sbResult1.AppendLine(@"                text = origin_legendary_leader_no_gov_change");
            sbResult1.AppendLine(@"                value = origin_legendary_leader_dictatorial");
            sbResult1.AppendLine(@"                value = origin_legendary_leader_imperial");
            sbResult1.AppendLine(@"            }");
            sbResult1.AppendLine(@"        }");
            sbResult1.AppendLine(@"        ethics = {");
            sbResult1.AppendLine(@"            NOR = {");
            sbResult1.AppendLine(@"                value = ethic_gestalt_consciousness");
            sbResult1.AppendLine(@"                value = ethic_fanatic_egalitarian");
            sbResult1.AppendLine(@"                value = ethic_fanatic_authoritarian");
            sbResult1.AppendLine(@"            }");
            sbResult1.AppendLine(@"        }");
            sbResult1.AppendLine(@"    }");
            sbResult1.AppendLine(@"    country_modifier = {");
            sbResult1.AppendLine(@"        commercial_pact_mult = 0.2");
            sbResult1.AppendLine(@"        empire_size_colonies_mult = 0.5");
            sbResult1.AppendLine(@"        external_leader_pool_add = 1");
            sbResult1.AppendLine(@"    }");
            sbResult1.AppendLine(@"    advanced_authority_swap = {");
            sbResult1.AppendLine(@"        name = ""auth_cyber_creed_democratic""");
            sbResult1.AppendLine(@"        description = ""auth_cyber_creed_democratic_desc""");
            sbResult1.AppendLine(@"        inherit_icon = no");
            sbResult1.AppendLine(@"        inherit_effects = no");
            sbResult1.AppendLine(@"        trigger = {");
            sbResult1.AppendLine(@"            is_scope_valid = yes");
            sbResult1.AppendLine(@"        }");
            sbResult1.AppendLine(@"        modifier = {");
            sbResult1.AppendLine(@"            pop_ethic_spiritualist_attraction_mult = 0.24");
            sbResult1.AppendLine(@"            pop_cyborg_happiness = 0.15");
            sbResult1.AppendLine(@"            pop_non_cyborg_happiness = -0.15");
            sbResult1.AppendLine(@"            pop_factions_produces_mult = 0.1");
            sbResult1.AppendLine(@"            all_technology_research_speed = 0.05");
            sbResult1.AppendLine(@"        }");
            sbResult1.AppendLine(@"    }");
            sbResult1.AppendLine(@"    advanced_authority_swap = {");
            sbResult1.AppendLine(@"        name = ""auth_cyber_creed_corporate""");
            sbResult1.AppendLine(@"        description = ""auth_cyber_creed_corporate_desc""");
            sbResult1.AppendLine(@"        inherit_icon = no");
            sbResult1.AppendLine(@"        inherit_effects = no");
            sbResult1.AppendLine(@"        trigger = {");
            sbResult1.AppendLine(@"            is_scope_valid = yes");
            sbResult1.AppendLine(@"            has_country_flag = cyber_creed_advanced_government");
            sbResult1.AppendLine(@"        }");
            sbResult1.AppendLine(@"        modifier = {");
            sbResult1.AppendLine(@"            pop_ethic_spiritualist_attraction_mult = 0.2");
            sbResult1.AppendLine(@"            pop_cyborg_happiness = 0.15");
            sbResult1.AppendLine(@"            pop_non_cyborg_happiness = -0.15");
            sbResult1.AppendLine(@"            pop_factions_produces_mult = 0.1");
            sbResult1.AppendLine(@"            all_technology_research_speed = 0.05");
            sbResult1.AppendLine(@"        }");
            sbResult1.AppendLine(@"    }");
            sbResult1.AppendLine(@"    advanced_authority_swap = {");
            sbResult1.AppendLine(@"        name = ""auth_cyber_corporate_individualist""");
            sbResult1.AppendLine(@"        description = ""auth_cyber_corporate_individualist_desc""");
            sbResult1.AppendLine(@"        inherit_icon = no");
            sbResult1.AppendLine(@"        inherit_effects = no");
            sbResult1.AppendLine(@"        trigger = {");
            sbResult1.AppendLine(@"            is_scope_valid = yes");
            sbResult1.AppendLine(@"            has_country_flag = cyber_individualist");
            sbResult1.AppendLine(@"        }");
            sbResult1.AppendLine(@"        modifier = {");
            sbResult1.AppendLine(@"            custom_tooltip = auth_cyber_corporate_individualist_tt");
            sbResult1.AppendLine(@"            starbase_trade_protection_add = 15");
            sbResult1.AppendLine(@"            planet_crime_add = 20");
            sbResult1.AppendLine(@"            pop_lifestyle_trade_value_add = 1");
            sbResult1.AppendLine(@"            pop_cyborg_happiness = 0.1");
            sbResult1.AppendLine(@"            show_only_custom_tooltip = no");
            sbResult1.AppendLine(@"        }");
            sbResult1.AppendLine(@"    }");
            sbResult1.AppendLine(@"    advanced_authority_swap = {");
            sbResult1.AppendLine(@"        name = ""auth_cyber_corporate_collectivist""");
            sbResult1.AppendLine(@"        description = ""auth_cyber_corporate_collectivist_desc""");
            sbResult1.AppendLine(@"        inherit_icon = no");
            sbResult1.AppendLine(@"        inherit_effects = no");
            sbResult1.AppendLine(@"        trigger = {");
            sbResult1.AppendLine(@"            is_scope_valid = yes");
            sbResult1.AppendLine(@"            has_country_flag = cyber_collectivist");
            sbResult1.AppendLine(@"        }");
            sbResult1.AppendLine(@"        modifier = {");
            sbResult1.AppendLine(@"            custom_tooltip = auth_cyber_corporate_collectivist_tt");
            sbResult1.AppendLine(@"            starbase_trade_protection_add = 15");
            sbResult1.AppendLine(@"            planet_crime_add = 20");
            sbResult1.AppendLine(@"            council_agenda_progress_speed = 0.25");
            sbResult1.AppendLine(@"            show_only_custom_tooltip = no");
            sbResult1.AppendLine(@"        }");
            sbResult1.AppendLine(@"    }");
            sbResult1.AppendLine(@"    advanced_authority_swap = {");
            sbResult1.AppendLine(@"        name = ""auth_synth_corporate_physical""");
            sbResult1.AppendLine(@"        description = ""auth_synth_corporate_physical_desc""");
            sbResult1.AppendLine(@"        inherit_icon = no");
            sbResult1.AppendLine(@"        inherit_effects = no");
            sbResult1.AppendLine(@"        trigger = {");
            sbResult1.AppendLine(@"            is_scope_valid = yes");
            sbResult1.AppendLine(@"            has_country_flag = synth_physical");
            sbResult1.AppendLine(@"        }");
            sbResult1.AppendLine(@"        modifier = {");
            sbResult1.AppendLine(@"            custom_tooltip = auth_synth_corporate_physical_tt");
            sbResult1.AppendLine(@"            trade_value_mult = 0.05");
            sbResult1.AppendLine(@"            show_only_custom_tooltip = no");
            sbResult1.AppendLine(@"        }");
            sbResult1.AppendLine(@"    }");
            sbResult1.AppendLine(@"    advanced_authority_swap = {");
            sbResult1.AppendLine(@"        name = ""auth_synth_corporate_virtual""");
            sbResult1.AppendLine(@"        description = ""auth_synth_corporate_virtual_desc""");
            sbResult1.AppendLine(@"        inherit_icon = no");
            sbResult1.AppendLine(@"        inherit_effects = no");
            sbResult1.AppendLine(@"        trigger = {");
            sbResult1.AppendLine(@"            is_scope_valid = yes");
            sbResult1.AppendLine(@"            has_country_flag = synth_virtual");
            sbResult1.AppendLine(@"        }");
            sbResult1.AppendLine(@"        modifier = {");
            sbResult1.AppendLine(@"            custom_tooltip = auth_synth_corporate_virtual_tt");
            sbResult1.AppendLine(@"            intel_decryption_add = 1");
            sbResult1.AppendLine(@"            show_only_custom_tooltip = no");
            sbResult1.AppendLine(@"        }");
            sbResult1.AppendLine(@"    }");
            sbResult1.AppendLine(@"    tags = {");
            sbResult1.AppendLine(@"        AUTHORITY_SUCCESSION_TYPE");
            sbResult1.AppendLine(@"        AUTHORITY_OLIGARCHIC_SUCCESSION");
            sbResult1.AppendLine(@"    }");
            sbResult1.AppendLine(@"}");
            sbResult1.AppendLine(@"");

            code.Trim().Should().Be(sbResult1.ToString().Trim());

            code = parser.MergeTopLevel(code.SplitOnNewLine(), "test.txt", "advanced_authority_swap", sb3.ToString().SplitOnNewLine(), true);

            var sbResult2 = new StringBuilder();
            sbResult2.AppendLine(@"auth_corporate = {");
            sbResult2.AppendLine(@"    election_term_years = 20");
            sbResult2.AppendLine(@"    election_type = oligarchic");
            sbResult2.AppendLine(@"    can_have_emergency_elections = yes");
            sbResult2.AppendLine(@"    max_election_candidates = 4");
            sbResult2.AppendLine(@"    localization_postfix = corporate");
            sbResult2.AppendLine(@"    has_agendas = yes");
            sbResult2.AppendLine(@"    ruler_council_position = councilor_ruler_corporate");
            sbResult2.AppendLine(@"    potential = {");
            sbResult2.AppendLine(@"        country_type = {");
            sbResult2.AppendLine(@"            NOT = {");
            sbResult2.AppendLine(@"                value = primitive");
            sbResult2.AppendLine(@"            }");
            sbResult2.AppendLine(@"        }");
            sbResult2.AppendLine(@"    }");
            sbResult2.AppendLine(@"    playable = {");
            sbResult2.AppendLine(@"        has_megacorp = yes");
            sbResult2.AppendLine(@"    }");
            sbResult2.AppendLine(@"    possible = {");
            sbResult2.AppendLine(@"        origin = {");
            sbResult2.AppendLine(@"            NOR = {");
            sbResult2.AppendLine(@"                text = origin_legendary_leader_no_gov_change");
            sbResult2.AppendLine(@"                value = origin_legendary_leader_dictatorial");
            sbResult2.AppendLine(@"                value = origin_legendary_leader_imperial");
            sbResult2.AppendLine(@"            }");
            sbResult2.AppendLine(@"        }");
            sbResult2.AppendLine(@"        ethics = {");
            sbResult2.AppendLine(@"            NOR = {");
            sbResult2.AppendLine(@"                value = ethic_gestalt_consciousness");
            sbResult2.AppendLine(@"                value = ethic_fanatic_egalitarian");
            sbResult2.AppendLine(@"                value = ethic_fanatic_authoritarian");
            sbResult2.AppendLine(@"            }");
            sbResult2.AppendLine(@"        }");
            sbResult2.AppendLine(@"    }");
            sbResult2.AppendLine(@"    country_modifier = {");
            sbResult2.AppendLine(@"        commercial_pact_mult = 0.2");
            sbResult2.AppendLine(@"        empire_size_colonies_mult = 0.5");
            sbResult2.AppendLine(@"        external_leader_pool_add = 1");
            sbResult2.AppendLine(@"    }");
            sbResult2.AppendLine(@"    advanced_authority_swap = {");
            sbResult2.AppendLine(@"        name = ""auth_cyber_creed_democratic""");
            sbResult2.AppendLine(@"        description = ""auth_cyber_creed_democratic_desc""");
            sbResult2.AppendLine(@"        inherit_icon = no");
            sbResult2.AppendLine(@"        inherit_effects = no");
            sbResult2.AppendLine(@"        trigger = {");
            sbResult2.AppendLine(@"            is_scope_valid = yes");
            sbResult2.AppendLine(@"        }");
            sbResult2.AppendLine(@"        modifier = {");
            sbResult2.AppendLine(@"            pop_ethic_spiritualist_attraction_mult = 0.24");
            sbResult2.AppendLine(@"            pop_cyborg_happiness = 0.15");
            sbResult2.AppendLine(@"            pop_non_cyborg_happiness = -0.15");
            sbResult2.AppendLine(@"            pop_factions_produces_mult = 0.1");
            sbResult2.AppendLine(@"            all_technology_research_speed = 0.05");
            sbResult2.AppendLine(@"        }");
            sbResult2.AppendLine(@"    }");
            sbResult2.AppendLine(@"    advanced_authority_swap = {");
            sbResult2.AppendLine(@"        name = ""auth_cyber_creed_corporate""");
            sbResult2.AppendLine(@"        description = ""auth_cyber_creed_corporate_desc""");
            sbResult2.AppendLine(@"        inherit_icon = no");
            sbResult2.AppendLine(@"        inherit_effects = no");
            sbResult2.AppendLine(@"        trigger = {");
            sbResult2.AppendLine(@"            is_scope_valid = yes");
            sbResult2.AppendLine(@"            has_country_flag = cyber_creed_advanced_government");
            sbResult2.AppendLine(@"        }");
            sbResult2.AppendLine(@"        modifier = {");
            sbResult2.AppendLine(@"            pop_ethic_spiritualist_attraction_mult = 0.2");
            sbResult2.AppendLine(@"            pop_cyborg_happiness = 0.15");
            sbResult2.AppendLine(@"            pop_non_cyborg_happiness = -0.15");
            sbResult2.AppendLine(@"            pop_factions_produces_mult = 0.1");
            sbResult2.AppendLine(@"            all_technology_research_speed = 0.05");
            sbResult2.AppendLine(@"        }");
            sbResult2.AppendLine(@"    }");
            sbResult2.AppendLine(@"    advanced_authority_swap = {");
            sbResult2.AppendLine(@"        name = ""auth_cyber_corporate_individualist""");
            sbResult2.AppendLine(@"        description = ""auth_cyber_corporate_individualist_desc""");
            sbResult2.AppendLine(@"        inherit_icon = no");
            sbResult2.AppendLine(@"        inherit_effects = no");
            sbResult2.AppendLine(@"        trigger = {");
            sbResult2.AppendLine(@"            is_scope_valid = yes");
            sbResult2.AppendLine(@"            has_country_flag = cyber_individualist");
            sbResult2.AppendLine(@"        }");
            sbResult2.AppendLine(@"        modifier = {");
            sbResult2.AppendLine(@"            custom_tooltip = auth_cyber_corporate_individualist_tt");
            sbResult2.AppendLine(@"            starbase_trade_protection_add = 15");
            sbResult2.AppendLine(@"            planet_crime_add = 20");
            sbResult2.AppendLine(@"            pop_lifestyle_trade_value_add = 1");
            sbResult2.AppendLine(@"            pop_cyborg_happiness = 0.1");
            sbResult2.AppendLine(@"            show_only_custom_tooltip = no");
            sbResult2.AppendLine(@"        }");
            sbResult2.AppendLine(@"    }");
            sbResult2.AppendLine(@"    advanced_authority_swap = {");
            sbResult2.AppendLine(@"        name = ""auth_cyber_corporate_collectivist""");
            sbResult2.AppendLine(@"        description = ""auth_cyber_corporate_collectivist_desc""");
            sbResult2.AppendLine(@"        inherit_icon = no");
            sbResult2.AppendLine(@"        inherit_effects = no");
            sbResult2.AppendLine(@"        trigger = {");
            sbResult2.AppendLine(@"            is_scope_valid = yes");
            sbResult2.AppendLine(@"            has_country_flag = cyber_collectivist");
            sbResult2.AppendLine(@"        }");
            sbResult2.AppendLine(@"        modifier = {");
            sbResult2.AppendLine(@"            custom_tooltip = auth_cyber_corporate_collectivist_tt");
            sbResult2.AppendLine(@"            starbase_trade_protection_add = 15");
            sbResult2.AppendLine(@"            planet_crime_add = 20");
            sbResult2.AppendLine(@"            council_agenda_progress_speed = 0.25");
            sbResult2.AppendLine(@"            show_only_custom_tooltip = no");
            sbResult2.AppendLine(@"        }");
            sbResult2.AppendLine(@"    }");
            sbResult2.AppendLine(@"    advanced_authority_swap = {");
            sbResult2.AppendLine(@"        name = ""auth_synth_corporate_physical""");
            sbResult2.AppendLine(@"        description = ""auth_synth_corporate_physical_desc""");
            sbResult2.AppendLine(@"        inherit_icon = no");
            sbResult2.AppendLine(@"        inherit_effects = no");
            sbResult2.AppendLine(@"        trigger = {");
            sbResult2.AppendLine(@"            is_scope_valid = yes");
            sbResult2.AppendLine(@"            has_country_flag = synth_physical");
            sbResult2.AppendLine(@"        }");
            sbResult2.AppendLine(@"        modifier = {");
            sbResult2.AppendLine(@"            custom_tooltip = auth_synth_corporate_physical_tt");
            sbResult2.AppendLine(@"            trade_value_mult = 0.05");
            sbResult2.AppendLine(@"            show_only_custom_tooltip = no");
            sbResult2.AppendLine(@"        }");
            sbResult2.AppendLine(@"    }");
            sbResult2.AppendLine(@"    advanced_authority_swap = {");
            sbResult2.AppendLine(@"        name = ""auth_synth_corporate_virtual""");
            sbResult2.AppendLine(@"        description = ""auth_synth_corporate_virtual_desc""");
            sbResult2.AppendLine(@"        inherit_icon = no");
            sbResult2.AppendLine(@"        inherit_effects = no");
            sbResult2.AppendLine(@"        trigger = {");
            sbResult2.AppendLine(@"            is_scope_valid = yes");
            sbResult2.AppendLine(@"            has_country_flag = synth_virtual");
            sbResult2.AppendLine(@"        }");
            sbResult2.AppendLine(@"        modifier = {");
            sbResult2.AppendLine(@"            custom_tooltip = auth_synth_corporate_virtual_tt");
            sbResult2.AppendLine(@"            intel_decryption_add = 1");
            sbResult2.AppendLine(@"            show_only_custom_tooltip = no");
            sbResult2.AppendLine(@"        }");
            sbResult2.AppendLine(@"    }");
            sbResult2.AppendLine(@"    tags = {");
            sbResult2.AppendLine(@"        AUTHORITY_SUCCESSION_TYPE");
            sbResult2.AppendLine(@"        AUTHORITY_OLIGARCHIC_SUCCESSION");
            sbResult2.AppendLine(@"    }");
            sbResult2.AppendLine(@"    advanced_authority_swap = {");
            sbResult2.AppendLine(@"        name = ""auth_cyber_creed_democratic2""");
            sbResult2.AppendLine(@"        description = ""auth_cyber_creed_democratic_desc2""");
            sbResult2.AppendLine(@"        inherit_icon = no");
            sbResult2.AppendLine(@"        inherit_effects = no");
            sbResult2.AppendLine(@"        trigger = {");
            sbResult2.AppendLine(@"            is_scope_valid = yes");
            sbResult2.AppendLine(@"        }");
            sbResult2.AppendLine(@"        modifier = {");
            sbResult2.AppendLine(@"            pop_ethic_spiritualist_attraction_mult = 0.3");
            sbResult2.AppendLine(@"            pop_cyborg_happiness = 0.15");
            sbResult2.AppendLine(@"            pop_non_cyborg_happiness = -0.15");
            sbResult2.AppendLine(@"            pop_factions_produces_mult = 0.1");
            sbResult2.AppendLine(@"            all_technology_research_speed = 0.05");
            sbResult2.AppendLine(@"        }");
            sbResult2.AppendLine(@"    }");
            sbResult2.AppendLine(@"}");
            sbResult2.AppendLine(@"");

            code.Trim().Should().Be(sbResult2.ToString().Trim());
        }
    }
}
