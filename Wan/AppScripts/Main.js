/* main: startup script creates the 'formBuilder' module and adds custom Ng directives */

window.wan = angular.module('wan', ['ngRoute', 'ngResource']);

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
        when('/login/:returnUrl', { templateUrl: 'AppScripts/Templates/UserLogin.html', controller: 'UserLoginCtrl' }).
        when('/signup', { templateUrl: 'AppScripts/Templates/UserSignup.html', controller: 'UserSignupCtrl' }).
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
    .directive('onEnter', function() {
        return function(scope, element, attrs) {
            element.bind("keydown keypress", function(event) {
                if (event.which === 13) {
                    scope.$apply(function() {
                        scope.$eval(attrs.onEnter);
                    });

                    event.preventDefault();
                }
            });
        };
    })
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