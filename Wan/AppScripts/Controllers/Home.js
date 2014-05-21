
wan.controller('HomeCtrl',
    ['$scope', 'datacontext',
    function ($scope, datacontext) {

        $scope.title = "Home";

        $scope.groups = [];
        
        $scope.groups = datacontext.getAllGroups().query();
        
        

        //$scope.refresh = function () {
        //    $scope.formDefinitionSets = datacontext.formDefinitionSets().query();
        //};

        //$scope.viewFormDefinitionVersion = function (formDefinitionModel) {
        //    window.location = '/#newForm/' + '?formDefinitionId=' + formDefinitionModel.id;
        //};


    }]);