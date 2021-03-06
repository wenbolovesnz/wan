﻿/* datacontext: data access and model management layer */

// create and add datacontext to the Ng injector
// constructor function relies on Ng injector
// to provide service dependencies
wan.factory('datacontext',
    ['Q', '$resource','$cacheFactory',
    function (Q, $resource, $cacheFactory) {

        var clientData = $cacheFactory('cachedData');

        clientData.put('groups', []);
        clientData.put('users', []);
        clientData.put('sponsors', []);

        function getAllGroups() {
            return $resource('api/Group/:groupId', {groupId: '@id'}, {
                query: { method: 'GET', isArray: true }
            });
        }
        
        function createGroup() {
            return $resource('api/Group/Create', {}, {
                create: { method: 'Post' }
            });
        }
        
        function updateGroup() {
            return $resource('api/Group/UpdateGroup', {}, {
                update: { method: 'Post' }
            });
        }
        
        function events() {
            return $resource('api/Event/:eventId/user/:userId', { eventId: '@id', userId: '@userId' });
        }
        
        function joinGroupRequest() {
            return $resource('api/JoinGroupRequest/:id', {id: '@id'}, {
                update:{ method: 'Post'}
            });
        }

        function personalMessage() {
            return $resource('api/PersonalMessage/:id', { id: '@id' }, {
                update: { method: 'Post' }
            });
        }

        function event() {
            return $resource('api/Event/:id', { id: '@id' }, {
                update: { method: 'Post' }
            });
        }

        function user() {
            return $resource('api/User/:id', { id: '@id' }, {
                update: { method: 'Post' }
            });
        }

        function sponsor() {
            return $resource('api/Sponsor/:id', { id: '@id' }, {
                query: { method: 'GET', isArray: true }
            });
        }
                        
        return {
            sponsor:sponsor,
            getAllGroups: getAllGroups,
            createGroup: createGroup,
            clientData: clientData,
            updateGroup: updateGroup,
            events: events,
            event: event,
            user: user,
            personalMessage: personalMessage,
            joinGroupRequest: joinGroupRequest
        };

    }]);

wan.factory('userService', [function () {
    
    var sdo = {
        isLogged: false,
        username: '',
        id: 0,
        dob: null,
        city: "",
        nickName: "",
        createdDate: "",
        aboutMe: "",
        userImage: null,
        personalMessages: [],
        
        isCurrentUserManager: function (username, users) {
            var managerRoleFound = _.find(users, function(user) {
                return user.userName == username && user.isGroupManager == true;
            });

            return managerRoleFound;
        }
    };
    
    return sdo;
}]);


wan.factory('docTitleService', ['$window', function ($window) {

    var isCurrentTab = true;
    $window.onblur = function() {
        isCurrentTab = false;
    };

    var setTitle = function(name) {
        $window.document.title = name;
    };
    
    var setOriginalValue = function() {
        $window.document.title = 'Join Me';
    };
    
    return {
        isCurentTab: isCurrentTab,
        setIsCurrentTab: function(value) {
            isCurrentTab = value;
            if (value == true) {
                setOriginalValue();
            }
        },
        setDocTitle: setTitle
    };
}]);


