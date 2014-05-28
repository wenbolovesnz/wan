
wan.controller('CreateGroupCtrl',
    ['$scope', 'datacontext', 'hub','userService','$location',
    function ($scope, datacontext, hub, userService, $location) {
        
        if (!userService.isLogged) {
            var currentPath = $location.path();
            //window.location = '/login' + currentPath;
            
            $location.path('login' + currentPath);
        }
        $scope.groupName = "";

        $scope.description = '';

        $scope.saveGroup = function () {
            var a = $scope.groupName;
            var b = $scope.description;

            datacontext.createGroup().create({groupName: a, description: b}, function (data) {
                var c = data;
                hub.server.newGroup(c);
                $location.path('home');
            });
        };
        
       
    }]);