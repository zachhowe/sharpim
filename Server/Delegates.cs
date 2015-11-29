using SharpIMServer.Core;
using SharpIMServer.Events;

namespace SharpIMServer
{
    // User delegates
    public delegate void UserGroupMessage(object sender, GroupMessageEventArgs e);

    public delegate void UserAuthenticated(object sender);

    public delegate void UserStatusRequest(object sender, string user);

    public delegate void UserSignOnTimeRequest(object sender, string from);

    public delegate void UserQuit(object sender);

    public delegate void UserStatus(object sender, UserStatusEventArgs e);

    public delegate void UserMessage(object sender, UserMessageEventArgs e);

    public delegate void UserJoinedGroup(string group, string user);

    public delegate void UserPartedGroup(string group, string user);

    public delegate void AdminKickedUser(string to, string from, string group);

    public delegate void AdminBannedUser(string to, string from, string group);

    // Group delegates
    public delegate void GroupSendMessage(object sender, GroupSendMessageEventArgs e);

    public delegate void GroupSendUserJoined(string to, string user, string group);

    public delegate void GroupSendUserParted(string to, string user, string group);

    public delegate void GroupSendUserKicked(string to, string target, string from, string group, string reason);

    public delegate void GroupSendUserBanned(string to, string target, string from, string group, string reason);

    public delegate void GroupUserEventError(string to, string errormessage);

    public delegate void GroupSendGroupProperty(string to, string whoset, GroupProperty prop);
}
