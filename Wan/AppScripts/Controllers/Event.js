
wan.controller('EventCtrl',
    ['$scope', 'datacontext', '$routeParams','userService',
    function ($scope, datacontext, $routeParams, userService) {

        $scope.isCurrentUserManager = false;
        $scope.updating = false;
        $scope.isEdit = false;
                
        $scope.event = datacontext.event().get({id: $routeParams.eventId}, function(result) {
            $scope.isCurrentUserManager = _.find($scope.event.group.groupManagers, function (u) {
                                                    return u.id == userService.id;
            });
            
            $scope.isCurrentUserInGroup = _.find($scope.event.group.users, function (u) {
                return u.id == userService.id;
            });

            $scope.canCreateMessage = userService.isLogged && $scope.isCurrentUserInGroup;

        });

        $scope.updateEvent = function () {
            $scope.updating = true;
            $scope.event.$update(function() {
                $scope.updating = false;
                $scope.isEdit = false;
            });
        };

        $scope.createNewMessage = function () {
            $scope.updating = true;

            var newMessage = {
                id: 0,
                createdDate: new Date(),
                user: { userName: userService.username },
                message: $scope.message
            };

            $scope.event.eventMessages.push(newMessage);

            $scope.event.$update(function () {
                $scope.updating = false;
                $scope.message = "";
            });
        };

        $scope.showEdit = function() {
            $scope.isEdit = true;
        };

        $scope.cancelEdit = function() {
            $scope.isEdit = false;
        };


    }]);