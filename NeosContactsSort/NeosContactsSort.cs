//#define DEBUG // if true do a lot of debug spam

using BaseX;
using CloudX.Shared;
using FrooxEngine;
using HarmonyLib;
using NeosModLoader;
using System;

namespace NeosContactsSort
{
    public class NeosContactsSort : NeosMod
    {
        public override string Name => "NeosContactsSort";
        public override string Author => "runtime";
        public override string Version => "1.2.0";
        public override string Link => "https://github.com/zkxs/NeosContactsSort";

        public override void OnEngineInit()
        {
#if DEBUG
            Warn($"Extremely verbose debug logging is enabled in this build. This probably means runtime messed up and gave you a debug build.");
#endif
            Harmony harmony = new Harmony("net.michaelripley.NeosContactsSort");
            harmony.PatchAll();
        }

        [HarmonyPatch]
        private static class HarmonyPatches
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(FriendsDialog), "OnCommonUpdate", new Type[] { })]
            public static void FriendsDialogOnCommonUpdatePrefix(ref bool ___sortList, out bool __state)
            {
                // steal the sortList bool's value, and force it to false from Neos's perspective
                __state = ___sortList;
                ___sortList = false;
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(FriendsDialog), "OnCommonUpdate", new Type[] { })]
            public static void FriendsDialogOnCommonUpdatePostfix(bool __state, SyncRef<Slot> ____listRoot)
            {
                // if Neos would have sorted (but we prevented it)
                if (__state)
                {
                    // we need to sort
                    ____listRoot.Target.SortChildren((slot1, slot2) =>
                    {
                        FriendItem? component1 = slot1.GetComponent<FriendItem>();
                        FriendItem? component2 = slot2.GetComponent<FriendItem>();
                        Friend? friend1 = component1?.Friend;
                        Friend? friend2 = component2?.Friend;

                        // nulls go last
                        if (friend1 != null && friend2 == null) return -1;
                        if (friend1 == null && friend2 != null) return 1;
                        if (friend1 == null && friend2 == null) return 0;

                        // friends with unread messages come first
                        int messageComparison = -component1!.HasMessages.CompareTo(component2!.HasMessages);
                        if (messageComparison != 0) return messageComparison;

                        // sort by online status
                        int onlineStatusOrder = GetOrderNumber(friend1!).CompareTo(GetOrderNumber(friend2!));
                        if (onlineStatusOrder != 0) return onlineStatusOrder;

                        // neos bot comes first
                        if (friend1!.FriendUserId == "U-Neos" && friend2!.FriendUserId != "U-Neos") return -1;
                        if (friend2!.FriendUserId == "U-Neos" && friend1!.FriendUserId != "U-Neos") return 1;

                        // sort by name
                        return string.Compare(friend1!.FriendUsername, friend2!.FriendUsername, StringComparison.CurrentCultureIgnoreCase);
                    });

#if DEBUG
                    Debug("BIG FRIEND DEBUG:");
                    foreach (Slot slot in ____listRoot.Target.Children)
                    {
                        FriendItem? component = slot.GetComponent<FriendItem>();
                        Friend? friend = component?.Friend;
                        if (friend != null)
                        {
                            Debug($"  {GetOrderNumber(friend)}: \"{friend.FriendUsername}\" status={friend.FriendStatus} online={friend.UserStatus?.OnlineStatus} incoming={friend.IsAccepted}");
                        }
                    }
#endif
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(NeosUIStyle), nameof(NeosUIStyle.GetStatusColor), new Type[] { typeof(Friend), typeof(Engine) })]
            public static void NeosUIStyleGetStatusColorPostfix(Friend friend, Engine engine, ref color __result)
            {
                OnlineStatus onlineStatus = friend.UserStatus?.OnlineStatus ?? OnlineStatus.Offline;
                if (onlineStatus == OnlineStatus.Offline && friend.FriendStatus == FriendStatus.Accepted && !friend.IsAccepted)
                {
                    __result = color.Yellow;
                }
            }
        }

        // lower numbers appear earlier in the list
        private static int GetOrderNumber(Friend friend)
        {
            if (friend.FriendStatus == FriendStatus.Requested) // received requests
                return 0;
            OnlineStatus status = friend.UserStatus?.OnlineStatus ?? OnlineStatus.Offline;
            switch (status)
            {
                case OnlineStatus.Online:
                    return 1;
                case OnlineStatus.Away:
                    return 2;
                case OnlineStatus.Busy:
                    return 3;
                default: // Offline or Invisible
                    if (friend.FriendStatus == FriendStatus.Accepted && !friend.IsAccepted)
                    { // sent requests
                        return 4;
                    }
                    else if (friend.FriendStatus != FriendStatus.SearchResult)
                    { // offline or invisible
                        return 5;
                        // unsure how people with no relation, ignored, or blocked will appear... but they'll end up here too
                    }
                    else
                    { // search results always come last
                        return 6;
                    }
            }
        }
    }
}
