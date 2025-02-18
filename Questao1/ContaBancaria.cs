using System.Globalization;

namespace Questao1
{
    class ContaBancaria {

        public int Numero { get; }
        private string _titular;
        private double _saldo;
        private const double TAXA_SAQUE = 3.50;

        public ContaBancaria(int numero, string titular, double depositoInicial = 0)
        {
            Numero = numero;
            _titular = titular;
            _saldo = depositoInicial;
        }

        public string Titular
        {
            get => _titular;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _titular = value;
                }
            }
        }

        public double Saldo => _saldo;

        public void Deposito(double valor)
        {
            if (valor > 0)
            {
                _saldo += valor;
            }
        }

        public void Saque(double valor)
        {
            _saldo -= (valor + TAXA_SAQUE);
        }

        public override string ToString()
        {
            return $"Conta {Numero}, Titular: {_titular}, Saldo: $ {_saldo.ToString("F2", CultureInfo.InvariantCulture)}";
        }

    }
}
