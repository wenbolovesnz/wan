
wan.controller('GroupDetailsCtrl',
    ['$scope', 'datacontext', 'hub', '$location', '$routeParams','userService','docTitleService', '$window',
    function ($scope, datacontext, hub, $location, $routeParams, userService, docTitleService, $window) {
        $scope.testFunc = function() {
            console.log($scope.test);
        };
        $scope.homeTab = true;
        $scope.membersTab = false;
        $scope.eventsTab = false;
        $scope.sponsorsTab = false;
        $scope.photosTab = false;
        
        function setAllTabsFalse() {
            $scope.homeTab = false;
            $scope.membersTab = false;
            $scope.eventsTab = false;
            $scope.sponsorsTab = false;
            $scope.photosTab = false;
        }

        $scope.showHomeTab = function() {
            setAllTabsFalse();
            $scope.homeTab = true;
        };        
        $scope.showMemembersTab = function () {
            setAllTabsFalse();
            $scope.membersTab = true;
        };        
        $scope.showEventsTab = function () {
            setAllTabsFalse();
            $scope.eventsTab = true;
        };        
        $scope.showSponsorsTab = function () {
            setAllTabsFalse();
            $scope.sponsorsTab = true;
        };        
        $scope.showPhotosTab = function () {
            setAllTabsFalse();
            $scope.photosTab = true;
        };
        




        $scope.isUserInGroup = function (userName) {
            return _.find($scope.group.users, function (u) {
                return u.userName == userName;
            });
        };

        $scope.isEdit = false;

        $scope.editToggle = function() {
            $scope.isEdit = !$scope.isEdit;
        };
               
        $scope.group = _.find(datacontext.clientData.get('groups'), function (g) {
            return g.id == $routeParams.groupId;
        });

        $scope.newGroupName = $scope.group.groupName;
        
        $scope.newDescription = $scope.group.description;

        $scope.groupCreator = _.find($scope.group.users, function(u) {
            return u.id == $scope.group.createdById;
        });
        
        $scope.image = $scope.group.groupImage;
        
        $scope.url = $scope.image;
        $scope.uploadUrl = 'api/Group/UploadImage';

        $scope.data = {
            groupId: $scope.group.id
        };
        
        $scope.uploadCompletedCallBack = function (imageFile) {
            
            var group = _.find(datacontext.clientData.get('groups'), function (g) {
                return g.id == $routeParams.groupId;
            });

            group.groupImage = imageFile;
        };
               
        //$scope.isCurrentUserManager = userService.isCurrentUserManager(userService.username, $scope.group.users);
        $scope.isCurrentUserManager = _.find($scope.group.groupManagers, function(user) {
            return user.id == userService.id;
        });

        $scope.isUserGroupManager = function(user) {
            return _.find($scope.group.groupManagers, function(u) {
                return u.id == user.id;
            });
        };
        
        hub.on('newGroupMememberArrived', function (user) {
            if (!$scope.isUserInGroup(user.userName)) {
                $scope.group.users.push(user);
                $scope.$apply();
            }                       
        });
        
        hub.on('newGroupMessage', function (msg) {
            $scope.messages.push({ message: msg });
            $scope.$apply();
            $('#messageChanel').scrollTop(200);
            if (!docTitleService.isCurentTab) {
                docTitleService.setDocTitle("New Message!");
            }
        });

        $window.onfocus = function() {
            docTitleService.setIsCurrentTab(true);
        };
    
        $scope.messages = [];

        $scope.showJoinBtn = true;
        $scope.groupMessageContent = "";
        
        if (userService.isLogged) {
            if ($scope.isUserInGroup(userService.username)) {
                hub.server.joinGroup($scope.group);
                $scope.showJoinBtn = false;                
            }
        }
        
        $scope.joinGroup = function() {
            if (userService.isLogged) {
                //datacontext.updateGroup().update($scope.group, function(data) {
                //    hub.server.joinGroup($scope.group);
                //});
                
                $scope.group.$save(function (result) {
                    hub.server.joinGroup($scope.group);
                });
                
                $scope.showJoinBtn = false;                
            } else {
                var currentPath = $location.path();               
                $location.path('login' + currentPath);
            }
        };
        
        $scope.sendMessage = function () {
            hub.server.sendGroupMessage({
                GroupName: $scope.group.groupName,
                Message: $scope.groupMessageContent,
                SendName: userService.username
            });
            $scope.groupMessageContent = "";
        };

        $scope.groupUpdating = false;
        $scope.saveGroup = function () {
            $scope.groupUpdating = true;
            $scope.group.groupName = $scope.newGroupName;
            $scope.group.description = $scope.newDescription;

            $scope.group.$save(function(result) {
                $scope.editToggle();
                $scope.groupUpdating = false;
            });
        };

        $scope.removeUserFromGroup = function(user) {
            removeItemFromArray($scope.group.users, user);            
            $scope.group.$save();
        };

        $scope.giveManangerRole = function(user) {
            var newManager = {
                id: user.id
            };
            $scope.group.groupManagers.push(newManager);
            $scope.group.$save();
        };

        $scope.removeManangerRole = function(user) {
            var managerToRemove = _.find($scope.group.groupManagers, function(gm) {
                return gm.id == user.id;
            });
            removeItemFromArray($scope.group.groupManagers, managerToRemove);
            $scope.group.$save();
        };

        function removeItemFromArray (array, item) {
            var index = _.indexOf(array, item);
            if (index != -1) {
                array.splice(index, 1);
            }
        };
        
         
        //---event date
        $scope.eventDateTime = "";
        
        $scope.saveNewEvent = function () {
            $scope.groupUpdating = true;

            var newEvent = {                
                name: $scope.eventName,
                description: $scope.eventDescription,
                eventLocation: $scope.eventLocation,
                eventDateTime: $scope.eventDateTime
            };

            $scope.group.events.push(newEvent);
            
            $scope.group.$save(function (result) {                
                $scope.groupUpdating = false;
                var eventToUpdate = _.find(result.events, function(event) {
                    return event.name == newEvent.name;
                });
                newEvent.id = eventToUpdate.id;
                $scope.hideCreateEvent();
            });
        };

        $scope.showCreateEvent = function() {
            $scope.displayCreateEvent = true;
        };
        
        $scope.hideCreateEvent = function () {
            $scope.displayCreateEvent = false;
        };

        $scope.joinEvent = function (event) {
            if (userService.isLogged) {
                
                $scope.groups = datacontext.events().save(
                    { eventId: event.id, userId: userService.id },
                    event
                ).$promise.then(function(successResult) {
                    var user = {
                        id: userService.id,
                        userName: userService.username,
                        profileImage: userService.profileImage
                    };
                    event.users.push(user);                    
                }, function(errorResult) {
                    
                });
                
            } else {
                $location.path('login');
            }
        };

        $scope.userJoined = function(event) {
            var currentUserIn = _.find(event.users, function(user) {
                return user.id == userService.id;
            });

            return currentUserIn;
        };
    }]);