﻿$(document).ready(function () {
    var ordersClient = new OrdersClient()
    ordersClient.GetPaymentGateways(true, function (res) {
        if (res.Success == true) {
            var paymentMethods = "";
            if (res.PaymentGateways.length > 0) {
                for (var i = 0; i < res.PaymentGateways.length; i++) {
                    if (i == 0) {
                        paymentMethods = "<li class='active' paymentId='" + res.PaymentGateways[i].Id + "'>";
                    }
                    else {
                        paymentMethods += "<li paymentId='" + res.PaymentGateways[i].Id + "'>";
                    }
                    paymentMethods += "<a href='#" + res.PaymentGateways[i].Name + "' data-toggle='tab'>" + res.PaymentGateways[i].Name + "<span></span></a></li>"
                }
            }
            $("#UlListitemsPaymentMethods").html(paymentMethods);
        }
    });

    ordersClient.GetBankAccounts(true, function (res) {
        console.log(res);
        if (res.Success == true) {
            var bankAccounts = "";
            if (res.BankAccounts.length > 0) {
                for (var bank = 0; bank < res.BankAccounts.length; bank++) {
                    bankAccounts += "<option value=" + bank + " bank>" + res.BankAccounts[bank].BankName + "</option>";
                }

            }
            $("#ddlCashDepositingBankAccount,#ddlChequeDepositingBankAccount,#ddlpurchaseOrderDepositingBank,#ddlOnlineTransferBank").append(bankAccounts);

        }
    });

    ordersClient.GetOnlinePaymentGateways(true, function (res) {
        console.log(res);
        if (res.Success == true) {
            var onlinePaymentGateways = "";
            if (res.PaymentGateways.length > 0) {
                for (var bank = 0; bank < res.PaymentGateways.length; bank++) {
                    onlinePaymentGateways += "<option value=" + res.PaymentGateways[bank].Id + ">" + res.PaymentGateways[bank].Name + "</option>";
                }

            }
            $("#txtOnlineTransferThroughCCAvenueOnlinePaymentGateway").append(onlinePaymentGateways);

        }
    });
});

$("#btnPaymentMethod").click(function () {
    $("#btnOrderSummary").removeClass("tab-style-blue").addClass("tab-style-default");
    $("#btnPaymentMethod").addClass("tab-style-blue").removeClass("tab-style-default");
    $("#divIVORDate").hide();
    $("#divIVORDate1").show();
    return false;
});