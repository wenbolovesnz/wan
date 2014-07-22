
wan.controller('HomeCtrl',
    ['$scope', 'datacontext', 'hub','$location',
    function ($scope, datacontext, hub, $location) {

        $scope.title = "Home";

        $scope.groups = [];

        $scope.groups = datacontext.getAllGroups().query(function () {
            datacontext.clientData.put('groups', $scope.groups);
        });

        hub.client.showNewGroup = function onShowNewGroup(group) {
            var newGroup = {
                groupName: group.GroupName,
                description: group.Description,
                createdDate: group.CreatedDate,
                users: group.Users,
                id: group.Id
            };
            $scope.groups.push(newGroup);
            $scope.$apply();
        };

        $scope.groupDetails = function (groupId) {
            $location.path("groupDetails/" + groupId);
        };
        
        $.connection.hub.logging = true;
        $.connection.hub.start();

    }]);