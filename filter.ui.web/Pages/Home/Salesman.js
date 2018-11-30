angular.module('centerEndApp').controller('salesmanCtrl', function ($scope, $http, $timeout, $ocLazyLoad, $element, NgTableParams, toaster) {
    $scope.pageBase = "Saleman";
    $scope.current = {};
    $scope.EditTitle = "";
    $scope.condition = {};

    $scope.sub = [];
    var subTabs = [];

    $scope.subTabs2 = subTabs;


    $scope.add = function () {
        $scope.current.PlantFormType = 1;
        $scope.subTabs = $scope.subTabs2;
        $scope.sub.detail = "/Pages/Home/SalesmanAdd.html?v=" + new Date().getTime();
        $scope.EditMode = "新建";
        $scope.current = {};
        $element.find(".maintain" + $scope.pageBase + "").modal({
            backdrop: false
        });
    };

    $scope.edit = function (item) {
        $scope.subTabs = $scope.subTabs2;
        $scope.sub.detail = "/Pages/Home/SalesmanEdit.html?v=" + new Date().getTime();
        $scope.EditMode = "维护";
        $scope.doGet("/Saleman/Info/"+ item.Id, null, function (data) {
            $scope.current = data.Data;
        });

        $element.find(".maintain" + $scope.pageBase + "").modal({
            backdrop: false
        });
    };

    $scope.save = function (item) {
        $scope.doPost("/Saleman/Save", item, function (data) {
            if (data.Result) {
                $scope.tableParams.reload();
                $element.find(".maintain" + $scope.pageBase).modal("hide");
            } else {
                toaster.warning(data.Message);
            }
        });
    };

    $scope.delete = function () {
        var items = [];
        for (var k in $scope.checkboxes.items) {
            if ($scope.checkboxes.items.hasOwnProperty(k)) {
                if ($scope.checkboxes.items[k]) {
                    items.push(parseInt(k));
                }
            }
        }

        if (items.length == 0) {
            toaster.warning("未选择删除项");
            return;
        }

        if (confirm("删除数据无法恢复，是否确认?")) {
            var join = items.join();

            $scope.doPost("/Saleman/Delete?ids=" + join, null, function (response) {
                if (response.Result) {
                    $scope.tableParams.reload();
                } else {
                    toaster.warning(response.Message);
                }
            });
        }
    };

    $scope.checkboxes = { 'checked': false, items: {} };

    $scope.search = function () {
        $scope.tableParams.$params.page = 1;
        $scope.tableParams.reload();
    };

    $scope.reset = function () {
        $scope.condition = {};
        $scope.tableParams.$params.page = 1;
        $scope.tableParams.reload();
    };

    $scope.tableParams = new NgTableParams({
        page: 1,
        count: 10
    }, {
            total: 99,
            getData: function ($defer, params) {
                $scope.checkboxes = { 'checked': false, items: {} };
                $scope.doGet("/Saleman/GetPagedSalemans?page=" + params.page() + "&size=" + params.count(), { params: $scope.condition }
                    , function (data) {
                        $timeout(function () {
                            params.total(data.Data.TotalCount);
                            $defer.resolve(data.Data.Items);
                        }, 500);
                    });
            }
        });

    $scope.endExtend = function () {
        $scope.tableParams.reload();
    };

    $scope.export = function () {
        var items = [];
        for (var k in $scope.checkboxes.items) {
            if ($scope.checkboxes.items.hasOwnProperty(k)) {
                if ($scope.checkboxes.items[k]) {
                    items.push(parseInt(k));
                }
            }
        }
        if (items.length == 0) {
            toaster.warning("请选择业务员");
            return;
        }

        document.location.href = "/Saleman/ExportSalemanAllShops?id=" + items[0];
        $scope.tableParams.reload();
    };
});