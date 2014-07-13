
wan.controller('EventCtrl',
    ['$scope', 'datacontext', '$routeParams',
    function ($scope, datacontext, $routeParams) {
        
        $scope.event = datacontext.event().get({id: $routeParams.eventId}, function(result) {
            var a = result;
        });

    }]);