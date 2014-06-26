
wan.controller('UserLoginCtrl',
    ['$scope', 'hub', 'userService', '$http', '$location',
function ($scope, hub, userService, $http, $location) {

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
                        window.location = "";                        
                    }
                })
                .error(function (data, status, headers, config) {
                    
                });
        };
    
        $scope.signup = function () {
            $location.path('signup');
        };

}]);

