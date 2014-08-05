wan.controller('MenuAfterCtrl',
    ['$scope', 'datacontext', 'userService', '$http', 'hub',
    function ($scope, datacontext, userService, $http, hub) {

        hub.on('newGroupMememberArrived', function (user) {
            
            var group = _.find(datacontext.clientData.get('groups'), function (g) {
                return g.id == user.groupId;
            });
            
            var isCurrentUserManager = _.find(group.groupManagers, function (u) {
                return u.id == userService.id;
            });
            
            if (isCurrentUserManager) {
                userService.messages.push(user);
                $scope.$apply();
            }
        });

        hub.on('newPersonalMessage', function (message) {
                if (message.User.Id == userService.id) {
                    userService.messages.push(message);
                    $scope.$apply();
                }                
        });

        $scope.user = userService;
        
        $http({ method: 'GET', url: '/Account/JoinGroupRequest' }).
              success(function (data, status, headers, config) {
                  $scope.user.messages = data.messages;

                  datacontext.personalMessage().query(function (result) {
                      _.each(result, function(a) {
                          $scope.user.personalMessages.push(a);
                      });                      
                  });

                  // this callback will be called asynchronously
                  // when the response is available
              }).
              error(function (data, status, headers, config) {
                  // called asynchronously if an error occurs
                  // or server returns response with an error status.
              });



        $scope.logout = function () {
            $http({ method: 'POST', url: '/Account/LogOff' }).success(function (data, status, headers, config) {
                location.reload();
            });
        };


    }]);