wan.controller('EventsCtrl',
    ['$scope', 'datacontext', 'userService', '$location',
    function ($scope, datacontext, userService, $location) {

        $scope.matchCase = "future";

        $scope.showAll = false;
        $scope.showFuture = true;
        $scope.showPast = false;

        $scope.getAll = function() {
            $scope.resetAll();
            $scope.showAll = true;
            $scope.matchCase = "all";
        };
        
        $scope.getFuture = function () {
            $scope.resetAll();
            $scope.showFuture = true;
            $scope.matchCase = "future";
        };
        
        $scope.getPast = function () {
            $scope.resetAll();
            $scope.showPast = true;
            $scope.matchCase = "past";
        };

        $scope.resetAll = function() {
            $scope.showAll = false;
            $scope.showFuture = false;
            $scope.showPast = false;
        };
        
        
        $scope.goEventDetails = function() {
            console.log("fired go details");
        };

    }]);