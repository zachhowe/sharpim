namespace SharpIMClient
{
    public delegate void UserJoinedGroupEventHandler(object sender, string group, string user);

    public delegate void UserPartedGroupEventHandler(object sender, string group, string user);

    public delegate void UserKickedUserEventHandler(object sender, string to, string from, string group, string reason);

    public delegate void UserBannedUserEventHandler(object sender, string to, string from, string group, string reason);

    public delegate void GroupMessageEventHandler(object sender, string group, string user, string message);

    public delegate void UserStatusEventHandler(object sender, string user, BuddyStatus status);

    public delegate void UserMessageEventHandler(object sender, string user, string message);

    public delegate void ServerMessageEventHandler(object sender, string message);

    public delegate void BuddyListReceivedEventHandler(object sender);
}
