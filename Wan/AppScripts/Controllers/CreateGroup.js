
wan.controller('CreateGroupCtrl',
    ['$scope', 'datacontext',
    function ($scope, datacontext) {

        $scope.groupName = "";

        $scope.description = '';

        $scope.saveGroup = function () {
            var a = $scope.groupName;
            var b = $scope.description;
            
            datacontext.createGroup().create({groupName: a, description: b}, function () {
                window.location = '/#';
            });

        };
    }]);