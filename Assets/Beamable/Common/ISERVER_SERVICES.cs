#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Content;
using Beamable.Common.Inventory;

public interface ISERVER_SERVICES
{
   // public Task<string> get_starter_case_region();
   // public Task<DateTime> get_utc_install_time();
   // public float get_cash_scale_factor(long player_xp);
   // public Task<InventoryUpdateBuilder> get_player_xp_and_rewards_for_levelling_up_inventory_update_builder(
   //    long current_player_xp, int player_xp_reward);
   public Task<List<INVENTORY_OBJECT<TContent>>> get_items<TContent>(InventoryView inventory_view, string? content_id = null)
      where TContent : ItemContent, new();
   // public Task<IContentObject> get_content(string content_id);
   // public Task<bool> player_has_unlocked_red_stars();
   // public bool player_has_unlocked_leagues(long player_xp);
   // public bool player_has_unlocked_battle_mode(long player_xp);
   // public bool player_has_unlocked_battle_pass(long player_xp);
   // public bool player_has_unlocked_one_question_shootout(long player_xp);
   // public int get_player_level(long player_xp);
   // public Task<bool> player_is_battle_mode_rank_1();
   // public Task<bool> player_is_leagues_rank_1();
}
