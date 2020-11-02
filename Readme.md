Technologies:

* C#
* .Net Core
* MongoDB
* MsSql
* Redis

#### For Starting the server with docker-compose

* Fill sql db password in docker-compose.yml and (appsetting.json or appsetting.development.json(for development env))
```zsh
    docker-compose up --build
```

#### Your server will run at http://localhost:5000

#### For running test

* Test settings was adjusted only for Controller files. 

```zsh
    dotnet test /p:CollectCoverage=true /p:Include="[*]MessagingServiceApp.Controllers.*"
```

or with runsettings.xml

```zsh
    dotnet test --collect:"XPlat Code Coverage" --settings runsettings.xml /p:CollectCoverage=true
```

## API Documentation

### Account
#### Register - POST Method - http://localhost:5000/api/account/register

Body params:
* userName -> Required
* email -> Required
* password -> Required
              Min. 8 character 
              Must be digit-lowercase-uppercase-nonLetter 
              Must match with confirmPassword
* confirmPassword -> Required

```javascript
{
    "userName": "user1",
    "email": "user1@test.com",
    "password": "P@ssw0rd",
    "confirmPassword": "P@ssw0rd"
}
```

If result is success:
* Status: 201
```javascript
{
    "result": {
        "userName": "user1",
        "email": "user1@test.com"
    },
    "message": null,
    "success": true,
    "exception": null
}
```

If any inputs not incorrect:
* Status: 400
```javascript
{
    "result": {
        "ConfirmPassword": [
            "'ConfirmPassword' and 'Password' do not match."
        ]
    },
    "message": "One or more validation errors occurred.",
    "success": false,
    "exception": null
}
```

If user already exist:
* Status: 400
```javascript
{
    "result": null,
    "message": "User already exist",
    "success": false,
    "exception": null
}
```

#### Login - POST Method - http://localhost:3000/api/account/login

Body params:
* email -> Required
* password -> Required
              Min. 8 character 
              Must be digit-lowercase-uppercase-nonLetter 
              Must match with confirmPassword

```javascript
{
    "email": "user1@test.com",
    "password": "P@ssw0rd"
}
```

If result is success:
* Status: 200
```javascript
{
    "result": {
        "token": "......."
    },
    "message": null,
    "success": true,
    "exception": null
}
```

If password is invalid:
* Status: 400
```javascript
{
    "result": null,
    "message": "User password is wrong with email 'user1@test.com'",
    "success": false,
    "exception": null
}
```

If no user:
* Status: 404
```javascript
{
    "result": null,
    "message": "There is no user with email: 'user9@test.com'",
    "success": false,
    "exception": null
}
```

### Message
#### SendMessage - POST Method - http://localhost:5000/api/message/sendMessage

Header:
* Authorization must be with Bearer Token

Body params:
* contactUserName -> Required

```javascript
{
    "contactUserName": "user2",
    "messageText": "user1 --> user2 message"
}
```

If result is success:
* Status: 200
```javascript
{
    "result": {
        "messageText": "user1 --> user2 message",
        "createdAt": "2020-11-02T00:03:41.227258+03:00"
    },
    "message": null,
    "success": true,
    "exception": null
}
```

If contact user not exist:
* Status: 404
```javascript
{
    "result": null,
    "message": "There is no user with name: 'user9'",
    "success": false,
    "exception": null
}
```

If user already blocked with contact user:
* Status: 400
```javascript
{
    "result": null,
    "message": "User restricted for sending message to contact user",
    "success": false,
    "exception": null
}
```

#### MessageList - GET Method - http://localhost:3000/api/chat/messageList

Header:
* Authorization must be with Bearer Token

Body params:
* contactUserUserName -> required
* pageNumber -> required

```javascript
{
    "contactUserUserName": "user2",
    "pageNumber": "1"
}
```

If result is success:
* Status: 200
```javascript
{
    "result": [
        {
            "senderUserUserName": "user1@test.com",
            "contactUserUserName": "user2@test.com",
            "messageText": "user1 --> user2 message",
            "createdAt": "2020-11-01T15:21:32.143Z"
        },
        {
            "senderUserUserName": "user1@test.com",
            "contactUserUserName": "user2@test.com",
            "messageText": "user1 --> user2 message",
            "createdAt": "2020-11-01T15:23:36.029Z"
        },
        {
            "senderUserUserName": "user1@test.com",
            "contactUserUserName": "user2@test.com",
            "messageText": "user1 --> user2 message",
            "createdAt": "2020-11-01T21:05:45.696Z"
        }
    ],
    "message": null,
    "success": true,
    "exception": null
}
```

If contact user not exist:
* Status: 404
```javascript
{
    "result": null,
    "message": "There is no user with name: 'user9'",
    "success": false,
    "exception": null
}
```

### User
#### BlockUser - POST Method - http://localhost:5000/api/user/blockUser

* You can change per page message count with MessageItemAmountPerPage key in appsettings file

Header:
* Authorization must be with Bearer Token

Body params:
* userEmail -> required

```javascript
{
    "userEmail": "user1@test.com"
}
```

If result is success:
* Status: 200
```javascript
{
    "result": {
        "blockingUserUserName": "user2",
        "blockedUserUserName": "user1",
        "isBlocked": true,
        "createdAt": "2020-11-02T00:06:15.719687+03:00"
    },
    "message": null,
    "success": true,
    "exception": null
}
```

If contact user not exist:
* Status: 404
```javascript
{
    "result": null,
    "message": "There is no user with email: 'user9@test.com'",
    "success": false,
    "exception": null
}
```

If user already blocked:
* Status: 400
```javascript
{
    "result": null,
    "message": "User already blocked with email: 'user1@test.com'",
    "success": false,
    "exception": null
}
```

### General Server Error
* Status: 500
```javascript
{
    "result": null,
    "message": "An error occured",
    "success": false,
    "exception": null
}
```