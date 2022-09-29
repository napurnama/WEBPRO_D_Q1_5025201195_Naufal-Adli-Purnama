interface payment {
    Object status();
    List<Object> getPayments();
}

interface Bank : Payment {
    void initiatePayments();
}

interface Loan : Payment {
    void intiateLoanSettlement();
    void initiateRePayment();
}