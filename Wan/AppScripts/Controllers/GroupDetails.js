﻿
wan.controller('GroupDetailsCtrl',
    ['$scope', 'datacontext', 'hub', '$location', '$routeParams','userService','docTitleService', '$window',
    function ($scope, datacontext, hub, $location, $routeParams, userService, docTitleService, $window) {
        $scope.isUserInGroup = function (userName) {
            return _.find($scope.group.users, function (u) {
                return u.userName == userName;
            });
        };
               
        $scope.group = _.find(datacontext.clientData.get('groups'), function (g) {
            return g.id == $routeParams.groupId;
        });

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
               
        $scope.isCurrentUserManager = userService.isCurrentUserManager(userService.username, $scope.group.users);
        
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
                datacontext.updateGroup().update($scope.group, function(data) {
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




    }]);