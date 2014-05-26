
wan.controller('UserLoginCtrl',
    ['$scope', 'hub', 'userService', '$http', '$routeParams', '$location',
function ($scope, hub, userService, $http, $routeParams, $location) {

        $scope.emailAddress = "";

        $scope.password = '';

        $scope.login = function () {
            var loginModel = {
                UserName: $scope.emailAddress,
                Password: $scope.password
            };
            
            $http.post('/Account/LoginAjax', loginModel)
                .success(function (data, status, headers, config) {
                    if (data.status) {
                        userService.isLogged = true;
                        userService.username = data.userName;
                        if ($routeParams.returnUrl) {
                            $location.path($routeParams.returnUrl);                            
                        } else {
                            window.location = "";
                        }
                    }
                })
                .error(function (data, status, headers, config) {
                    
                });
        };
    
        $scope.signup = function () {
            $location.path('signup');
        };

}]);

