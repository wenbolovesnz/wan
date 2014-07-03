wan.controller('MenuAfterCtrl',
    ['$scope', 'datacontext', 'userService', '$http',
    function ($scope, datacontext, userService, $http) {
        $scope.user = userService;
        
        $http({ method: 'GET', url: '/Account/JoinGroupRequest' }).
              success(function (data, status, headers, config) {
                  $scope.user.messages = data.messages;
                  // this callback will be called asynchronously
                  // when the response is available
              }).
              error(function (data, status, headers, config) {
                  // called asynchronously if an error occurs
                  // or server returns response with an error status.
              });
    }]);