/* main: startup script creates the 'formBuilder' module and adds custom Ng directives */

window.wan = angular.module('wan', ['ngRoute', 'ngResource', 'ui.bootstrap']);

$.connection.hub.error(function(err) {
    console.log('An error occurred: ' + err);
});

wan.value('hub', $.connection.joinmeHub);

wan.value('Q', window.Q);

// Configure routes
wan.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.
        when('/', { templateUrl: 'AppScripts/Templates/Home.html', controller: 'HomeCtrl' }).
        when('/home', { templateUrl: 'AppScripts/Templates/Home.html', controller: 'HomeCtrl' }).
        when('/createGroup', { templateUrl: 'AppScripts/Templates/CreateGroup.html', controller: 'CreateGroupCtrl' }).
        when('/groupDetails/:groupId', { templateUrl: 'AppScripts/Templates/GroupDetails.html', controller: 'GroupDetailsCtrl' }).
        when('/myAccount', { templateUrl: 'AppScripts/Templates/MyAccount.html', controller: 'MyAccountCtrl' }).
        otherwise({ redirectTo: '/home' });
}]);

//#region Ng directives
/*  We extend Angular with custom data bindings written as Ng directives */
wan.directive('onFocus', function() {
    return {
        restrict: 'A',
        link: function(scope, elm, attrs) {
            elm.bind('focus', function() {
                scope.$apply(attrs.onFocus);
            });
        }
    };
})
    .directive('onBlur', function() {
        return {
            restrict: 'A',
            link: function(scope, elm, attrs) {
                elm.bind('blur', function() {
                    scope.$apply(attrs.onBlur);
                });
            }
        };
    })
    .directive('ngEnterkey', function () {
        return {
            restrict: 'A',
            link: function(scope, element, attr) {
                element.bind("keydown keypress", function (event) {
                    if (event.which === 13) {
                        scope.$apply(function () {
                            scope.$eval(attr.ngEnterkey);
                        });
                        event.preventDefault();
                    }
                });
            }                
        };
    })
    .directive('imageurl', function () {
        return {
            restrict: 'A',
            link: function (scope, element, attr) {                
                var imageUrl = '/azure' + scope.userdata.profileImage.slice(36) + "?width=50&height=50";
                element.attr("src", imageUrl);
            },
            scope: {
                userdata:"="
            }
        };
    })
    .directive('uploader', ['userService', function (userService) {
        return {
            restrict: 'E',
            templateUrl: 'AppScripts/Templates/Directives/Uploader.html',
            link: function (scope, element, attr) {

                var uploadBtn = element.find("input:button");
                uploadBtn.on('click', function () {
                    uploadBtn.attr("disabled", "disabled");
                    element.find('#spinner').show();
                    $.ajaxFileUpload(
                        {
                            url: scope.uploadurl,
                            secureuri: false,
                            fileElementId: element.find("input:file").attr('id'),
                            dataType: scope.datatype,
                            data: scope.fromdata,
                            success: function (data, status) {
                                var error = element.find(".error");
                                if (data.succeeded) {
                                    scope.image = data.imageFile;
                                    var imageElement = element.find("img");
                                    imageElement.attr("src", (data.imageFile));
                                    scope.callback(data.imageFile);
                                    error.hide();
                                } else {
                                    error.html('Error, your file type maybe not valid, please try again.');
                                    error.show();
                                }
                                uploadBtn.removeAttr('disabled');
                                element.find('#spinner').hide();
                            },
                            error: function (data, status, e) {
                                var error = element.find(".error");
                                error.html('Error, your file type maybe not valid, please try again.');
                                error.show();
                                element.find('#spinner').hide();
                                uploadBtn.removeAttr('disabled');
                            }
                        }
                    );
                });
            },
            scope: {
                image: "=",
                url: "=",
                display: "=",
                uploadurl: "=",
                callback: "=",
                fromdata: "=",
                datatype: "@"
            }
        };
    }])
    .directive('selectedWhen', function() {
        return function(scope, elm, attrs) {
            scope.$watch(attrs.selectedWhen, function(shouldBeSelected) {
                if (shouldBeSelected) {
                    elm.select();
                }
            });
        };
    });

if (!Modernizr.input.placeholder) {
    // this browser does not support HTML5 placeholders
    // see http://stackoverflow.com/questions/14777841/angularjs-inputplaceholder-directive-breaking-with-ng-model
    wan.directive('placeholder', function () {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function (scope, element, attr, ctrl) {

                var value;

                var placeholder = function () {
                    element.val(attr.placeholder);
                };
                var unplaceholder = function () {
                    element.val('');
                };

                scope.$watch(attr.ngModel, function (val) {
                    value = val || '';
                });

                element.bind('focus', function () {
                    if (value == '') unplaceholder();
                });

                element.bind('blur', function () {
                    if (element.val() == '') placeholder();
                });

                ctrl.$formatters.unshift(function (val) {
                    if (!val) {
                        placeholder();
                        value = '';
                        return attr.placeholder;
                    }
                    return val;
                });
            }
        };
    });
}
//#endregion 