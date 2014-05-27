
wan.controller('GroupDetailsCtrl',
    ['$scope', 'datacontext', 'hub', '$location', '$routeParams',
    function ($scope, datacontext, hub, $location, $routeParams) {
               
        $scope.group = _.find(datacontext.clientData.get('groups'), function (g) {
            return g.id == $routeParams.groupId;
        });
    }]);