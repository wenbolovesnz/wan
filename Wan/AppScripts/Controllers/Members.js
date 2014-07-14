wan.controller('MembersCtrl',
    ['$scope', 'datacontext', 'userService', '$location',
    function ($scope, datacontext, userService, $location) {

        $scope.goUserDetails = function (u) {
            $location.path('user/' + u.id);
        };

    }]);