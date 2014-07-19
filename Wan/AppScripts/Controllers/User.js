
wan.controller('UserCtrl',
    ['$scope', 'datacontext', '$routeParams',
    function ($scope, datacontext, $routeParams) {

        $scope.user = datacontext.user().get({ id: $routeParams.userId }, function (result) {
            $scope.$broadcast("picDownloaded");
        });

        $scope.$on("picDownloaded", function () {
            if ($scope.user.profileImage) {
                $("#usericon").attr("src", $scope.user.profileImage);
            }            
        });

    }]);