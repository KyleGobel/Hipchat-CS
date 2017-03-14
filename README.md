Hipchat-CS
==========

An easy to use C# wrapper on the V2 Hipchat API

Can be installed through nuget.

```
Install-Package Hipchat-CS
```

Not all methods are added yet, but if you find something you need, feel free to create a pull request, or request support for it in the issues.


**Getting Started**

Before you can use any of the methods you need an API key, or an auth token.

The easiest method is to login to hipchat, click on Account Settings, click on *API Access* and you should see your personal bearer token there

![Auth pic](http://i.imgur.com/UzievNL.png)

Using this method will access the api as yourself (all notifications/messages sent will be from you).

Once you have your api key, you can easily use the api

```csharp 
var client = new HipchatClient(apiKey);

//assume we have a room named 'My Room'
client.SendNotification("My Room", "This is your captain speaking");
```

All methods have several optional paramaters and or overloads, fully documented.  Check intellisense.

**Examples**

```cs
//will look for an environment variable named 'hipchat_auth_token',
//or you can pass it in via the constructor
var client = new HipchatClient();

//check source or intellisense for overloads
//create a default room with whoever the api key holder is as owner
HipchatCreateRoomResponse testRoom = client.CreateRoom("My Test Room");

//send a message to the created room with the green background color
client.SendNotification(testRoom.Id, "Hello from Api!", RoomColors.Green);

//delete the room, kicking everyone out
client.DeleteRoom(testRoom.Id);
```

For more examples check the IntegrationTests project files
