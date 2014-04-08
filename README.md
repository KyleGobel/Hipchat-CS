Hipchat-CS
==========

An easy to use C# wrapper on the V2 Hipchat API

Can be installed through nuget.

```
Install-Package Hipchat-CS
```

**Work in progress**

Finished Methods

- CreateRoom
- GetAllRooms
- SendNotification
- CreateWebHook


All methods have several optional paramaters, check intellisense or source for more usage.  These methods fully supported
thus far.


**Examples**

```cs
//will look in app.config for the appSetting 'hipchat_auth_token', 
//or you can pass it in via the constructor
var client = new HipchatClient();

//check source or intellisense for overloads
//create a default room with whoever the api key holder is as owner
HipchatCreateRoomResponse testRoom = client.CreateRoom("My Test Room");

//send a message to the created room with the green background color
client.SendNotification(testRoom.Id, "Hello from Api!", RoomColors.Green);
```
