using System;
using System.Collections.Generic;

namespace bank_system
{
    enum CustomerType
    {
        PHYSICAL_PERSON,
        COMPANY,
    }

    enum BankAccountType
    {
        SAVING, // sporici
        CREDIT, // uverovy
        MORTGAGE, // hypotecni
    }

    class Customer
    {

        public CustomerType customer_type;
        string customer_name;

        public override string ToString()
        {
            return this.customer_type.ToString() + ", name: " + this.customer_name;
        }

        public Customer(CustomerType customer_type, string customer_name)
        {
            this.customer_type = customer_type;
            this.customer_name = customer_name;
        }
    }

    class BankAccount
    {
        public Dictionary<BankAccountType, double> interests = new Dictionary<BankAccountType, double>
        {
            {BankAccountType.SAVING, 0.003},
            {BankAccountType.CREDIT, 0.015},
            {BankAccountType.MORTGAGE, 0.005},
        };

        public Customer customer;
        public double balance;
        public double interest;
        public int paid_months;
        public BankAccountType account_type;

        public override string ToString()
        {
            return "Account type: " + this.account_type.ToString() + ", customer: " + this.customer.ToString() + ", balance: " + this.balance + ", interest: " + this.interest + ", total months paid: " + this.paid_months;
        }

        public void Save(double amount)
        {
            this.balance += amount;
        }

        public bool Withdraw(double amount)
        {
            if (amount <= this.balance)
            {
                this.balance -= amount;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Based on an account's CustomerType, AccountType and balance in case of saving accounts computes: see "Returns:"
        /// </summary>
        /// <returns>[lower interest duration, ratio forgiven (e.g. 0.3 = 30 % forgiven)]</returns>
        public double[] ForgivenMonthsAndPercentage()
        {
            int paid_months = this.paid_months;
            CustomerType customer_type = this.customer.customer_type;
            BankAccountType account_type = this.account_type;
            switch (account_type)
            {
                case BankAccountType.SAVING:
                    if (100000 > this.balance && this.balance > 0)
                        return new double[] { 9999, 1 };
                    else
                        return new double[] { 0 - paid_months, 0 };
                case BankAccountType.CREDIT:
                    if (customer_type == CustomerType.PHYSICAL_PERSON)
                        return new double[]{3 - paid_months, 1};
                    else
                        return new double[] { 2 - paid_months, 1 };
                default: // BankAccountType.MORTGAGE
                    if (customer_type == CustomerType.COMPANY)
                        return new double[] { 12 - paid_months, 0.5 };
                    else
                        return new double[] { 6 - paid_months, 1 };
            }
        }

        public double Interest(double months)
        {
            double[] forgiven = this.ForgivenMonthsAndPercentage();
            double forgiven_months = Math.Max(forgiven[0], 0);
            double forgiven_ratio = forgiven[1];
            double interest = (1 - forgiven_ratio) * this.balance * this.interest * forgiven_months;
            months = Math.Max(months - forgiven_months, 0);
            interest += this.balance * this.interest * months;
            return interest;
        }
        public int IncrementPaidMonths(int months)
        {
            this.paid_months += months;
            return paid_months;
        }
    }
    
    class SavingAccount : BankAccount
    {
        public SavingAccount(CustomerType customer_type, string customer_name, double balance)
        {
            Customer _customer = new Customer(customer_type, customer_name);
            this.customer = _customer;
            this.balance = balance;
            this.interest = interests[BankAccountType.SAVING];
            this.paid_months = 0;
            this.account_type = BankAccountType.SAVING;
            Console.WriteLine("Account created: " + this.ToString());
        }

    }

    class CreditAccount : BankAccount
    {
        public CreditAccount(CustomerType customer_type, string customer_name, double balance)
        {
            Customer _customer = new Customer(customer_type, customer_name);
            this.customer = _customer;
            this.balance = balance;
            this.interest = interests[BankAccountType.CREDIT];
            this.paid_months = 0;
            this.account_type = BankAccountType.CREDIT;
            Console.WriteLine("Account created: " + this.ToString());
        }
    }

    class MortgageAccount : BankAccount
    {
        public MortgageAccount(CustomerType customer_type, string customer_name, double balance)
        {
            Customer _customer = new Customer(customer_type, customer_name);
            this.customer = _customer;
            this.balance = balance;
            this.interest = interests[BankAccountType.MORTGAGE];
            this.paid_months = 0;
            this.account_type = BankAccountType.MORTGAGE;
            Console.WriteLine("Account created: " + this.ToString());
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            SavingAccount GCHD = new SavingAccount(CustomerType.COMPANY, "Gymnázium Christiana Dopplera", 50000);
            Console.WriteLine(GCHD.balance);
            Console.WriteLine(GCHD.Interest(10));
            GCHD.Save(49000);
            Console.WriteLine(GCHD.Interest(10));
            GCHD.Save(1000);
            Console.WriteLine(GCHD.Interest(10));

            MortgageAccount Babis = new MortgageAccount(CustomerType.PHYSICAL_PERSON, "Andrej Babiš", 100000);
            Console.WriteLine(Babis.Interest(4));
            Console.WriteLine(Babis.Interest(8));
            Console.WriteLine(Babis.Interest(10));
            Babis.IncrementPaidMonths(3);
            Console.WriteLine(Babis.Interest(10));

            Console.ReadLine();
        }
    }
}
