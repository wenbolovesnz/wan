
wan.controller('CreateGroupCtrl',
    ['$scope', 'datacontext', 'hub',
    function ($scope, datacontext, hub) {

        $scope.groupName = "";

        $scope.description = '';

        $scope.saveGroup = function () {
            var a = $scope.groupName;
            var b = $scope.description;

            datacontext.createGroup().create({groupName: a, description: b}, function (data) {
                var c = data;
                hub.server.newGroup(c);
                window.location = '';               
            });
        };
                
    }]);