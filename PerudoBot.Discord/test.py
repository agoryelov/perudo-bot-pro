class BankAccount():
    def __init__(self) -> None:
        self.balance = 200

class Person():
    def __init__(self, name, age, bank_account) -> None:
        self.name = name
        self.age = age
        self.bank_account : BankAccount = bank_account
