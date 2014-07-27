﻿wan.controller('SponsorCtrl',
    ['$scope', 'datacontext', '$routeParams', 'userService',
    function ($scope, datacontext, $routeParams, userService) {

        $scope.updating = false;


        $scope.sponsor = _.find(datacontext.clientData.get('sponsors'), function (g) {
            return g.id == $routeParams.sponsorId;
        });

        $scope.image = $scope.sponsor.photoUrl;
        $scope.url = $scope.image != null ? $scope.image : '/content/images/defaultgroup.png';
        $scope.uploadUrl = 'Account';

        $scope.updateSponsor = function () {
            $scope.updating = true;
            $scope.event.$update(function () {
                $scope.updating = false;
                $scope.isEdit = false;
            });
        };

       

    }]);