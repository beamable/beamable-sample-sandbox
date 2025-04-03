#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Content;
using Beamable.Common.Inventory;
using Beamable.Microservices;
// using MongoDB.Driver;

public class SERVER_SERVICES : ISERVER_SERVICES
{
   private readonly Service service;

   public SERVER_SERVICES(Service service)
   {
      this.service = service;
   }
   
   public Task<List<INVENTORY_OBJECT<TCONTENT>>> get_items<TCONTENT>(InventoryView inventory_view, string? content_id = null)
      where TCONTENT : ItemContent, new()
   {
      return service.get_items<TCONTENT>(inventory_view, content_id);
   }
   //
   // public async Task<string> get_starter_case_region()
   // {
   //    var user_data = await service.get_user_data<USER_DATA>(Builders<USER_DATA>.Projection.Include(u => u.starter_case_region));
   //    return user_data.starter_case_region;
   // }
   //
   // public float get_cash_scale_factor(long player_xp)
   // {
   //    return service.get_cash_scale_factor(player_xp);
   // }
   //
   // public Task<InventoryUpdateBuilder> get_player_xp_and_rewards_for_levelling_up_inventory_update_builder(long current_player_xp, int player_xp_reward)
   // {
   //    return service.get_player_xp_and_rewards_for_levelling_up_inventory_update_builder(current_player_xp:current_player_xp, player_xp_reward:player_xp_reward);
   // }
   //
   // public async Task<DateTime> get_utc_install_time()
   // {
   //    var user_data = await service.get_user_data<USER_DATA>(Builders<USER_DATA>.Projection.Include(u => u.install_time_utc_time_ticks));
   //    var install_time = BASE_UTILITIES.get().get_utc_date_time(user_data.install_time_utc_time_ticks);
   //    return install_time;
   // }
   //
   // public Task<IContentObject> get_content(string content_id)
   // {
   //    return service.get_content(content_id);
   // }
   //
   // public Task<bool> player_has_unlocked_red_stars()
   // {
   //    return service.player_has_unlocked_red_stars();
   // }
   //
   // public bool player_has_unlocked_leagues(long player_xp)
   // {
   //    return service.player_has_unlocked_leagues(player_xp);
   // }
   //
   // public bool player_has_unlocked_battle_mode(long player_xp)
   // {
   //    return service.battle_mode_unlocked_for_player(player_xp);
   // }
   //
   // public bool player_has_unlocked_battle_pass(long player_xp)
   // {
   //    return service.battle_pass_unlocked_for_player(player_xp);
   // }
   //
   // public bool player_has_unlocked_one_question_shootout(long player_xp)
   // {
   //    return service.one_question_shootout_unlocked_for_player(player_xp);
   // }
   //
   // public Task<bool> player_is_battle_mode_rank_1()
   // {
   //    return service.player_is_battle_mode_rank_1();
   // }
   //
   // public Task<bool> player_is_leagues_rank_1()
   // {
   //    return service.player_is_leagues_rank_1();
   // }
   //
   // public int get_player_level(long player_xp)
   // {
   //    return service.get_player_level(player_xp);
   // }

}
