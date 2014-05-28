﻿
wan.controller('GroupDetailsCtrl',
    ['$scope', 'datacontext', 'hub', '$location', '$routeParams','userService',
    function ($scope, datacontext, hub, $location, $routeParams, userService) {
        $scope.isUserInGroup = function (userName) {
            return _.find($scope.group.users, function (u) {
                return u.userName == userName;
            });
        };
               
        $scope.group = _.find(datacontext.clientData.get('groups'), function (g) {
            return g.id == $routeParams.groupId;
        });
        

        hub.on('newGroupMememberArrived', function (user) {
            if (!$scope.isUserInGroup(user.userName)) {
                $scope.group.users.push(user);
                $scope.$apply();
            }
                       
        });
        
        hub.on('newGroupMessage', function (msg) {
            $scope.messages.push({ message: msg });
            $scope.$apply();
        });
    
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
                hub.server.joinGroup($scope.group);
                $scope.showJoinBtn = false;
                datacontext.updateGroup().update($scope.group);
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