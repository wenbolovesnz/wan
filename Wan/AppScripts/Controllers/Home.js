﻿
wan.controller('HomeCtrl',
    ['$scope', 'datacontext',
    function ($scope, datacontext) {

        $scope.title = "Home";

        $scope.groups = [];
        
        $scope.groups = datacontext.getAllGroups().query();


        //$scope.formDefinitionSets = datacontext.formDefinitionSets(true).query(function () {
        //    datacontext.cacheFormDefinitionSet($scope.formDefinitionSets);

        //    angular.forEach($scope.formDefinitionSets, function (formDefinitionSet, index) {
        //        angular.forEach(formDefinitionSet.formDefinitionModels, function (formDefinition, index) {
        //            if (formDefinition.isPublished == true) {
        //                $scope.publishedForms.push(formDefinition);
        //            }
        //        });
        //    });

        //});

        //$scope.refresh = function () {
        //    $scope.formDefinitionSets = datacontext.formDefinitionSets().query();
        //};

        //$scope.viewFormDefinitionVersion = function (formDefinitionModel) {
        //    window.location = '/#newForm/' + '?formDefinitionId=' + formDefinitionModel.id;
        //};


    }]);