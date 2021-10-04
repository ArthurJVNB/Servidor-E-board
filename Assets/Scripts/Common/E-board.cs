using System;
using System.IO.Ports;
using UnityEngine;
using System.IO;

namespace Assets.Scripts
{
    /// <summary>
    /// Representa um dispositivo Eboard.
    /// </summary>
    public class Eboard
    {
        /// <summary>
        /// Cria uma nova instância do dispositivo.
        /// </summary>
        /// <param name="port">O nome da porta serial que o dispositivo está conectado.</param>
        /// <param name="name">O nome do dispositivo.</param>
        /// <param name="battery">A bateria atual do dispositivo.</param>
        public Eboard(string port, string name, float battery)
        {
            this.Port = new SerialPort(port, 115200, Parity.None, 8, StopBits.One);
            this.Port.Handshake = Handshake.None;
            this.Port.ReadTimeout = 500;
            this.Port.WriteTimeout = 500;
            this.Port.DtrEnable = true;
            this.Bluetooth = false;
            this.Name = name;
            this.AverageBattery = battery;
            this.Battery = new float[50];
            for (int i = 0; i < 50; i++)
            {
                this.Battery[i] = battery;
            }
        }

        /// <summary>
        /// Cria uma nova instância do dispositivo.
        /// </summary>
        /// <param name="port">O nome da porta serial que o dispositivo está conectado.</param>
        /// <param name="bluetooth">Se o dispositivo se comunica através de bluetooth.</param>
        /// <param name="name">O nome do dispositivo.</param>
        /// <param name="battery">A bateria atual do dispositivo.</param>
        public Eboard(string port, bool bluetooth, string name, float battery)
        {
            if (bluetooth)
                this.Port = new SerialPort(port, 9600, Parity.None, 8, StopBits.One);
            else
                this.Port = new SerialPort(port, 115200, Parity.None, 8, StopBits.One);

            this.Port.Handshake = Handshake.None;
            this.Port.ReadTimeout = 500;
            this.Port.WriteTimeout = 500;
            this.Port.DtrEnable = true;
            this.Bluetooth = bluetooth;
            this.Name = name;
            this.AverageBattery = battery;
            this.Battery = new float[50];
            for (int i = 0; i < 50; i++)
            {
                this.Battery[i] = battery;
            }
        }

        /// <summary>
        /// Cria uma nova instância do dispositivo.
        /// </summary>
        /// <param name="port">A porta serial que o dispositivo está conectado.</param>
        /// <param name="name">O nome do dispositivo.</param>
        /// <param name="battery">A bateria atual do dispositivo.</param>
        public Eboard(SerialPort port, string name, float battery)
        {
            this.Port = port;
            this.Bluetooth = false;
            this.Name = name;
            this.AverageBattery = battery;
            this.Battery = new float[50];
            for (int i = 0; i < 50; i++)
            {
                this.Battery[i] = battery;
            }
        }

        /// <summary>
        /// Cria uma nova instância do dispositivo.
        /// </summary>
        /// <param name="port">A porta serial que o dispositivo está conectado.</param>
        /// <param name="bluetooth">Se o dispositivo se comunica através de bluetooth.</param>
        /// <param name="name">O nome do dispositivo.</param>
        /// <param name="battery">A bateria atual do dispositivo.</param>
        public Eboard(SerialPort port, bool bluetooth, string name, float battery)
        {
            this.Port = port;
            this.Bluetooth = bluetooth;
            this.Name = name;
            this.AverageBattery = battery;
            this.Battery = new float[50];
            for (int i = 0; i < 50; i++)
            {
                this.Battery[i] = battery;
            }
        }

        /// <summary>
        /// Estabelece a conexão com a porta serial.
        /// </summary>
        public void Connect()
        {
            if (!Port.IsOpen)
                Port.Open();
        }

        /// <summary>
        /// Encerra a conexão com a porta serial.
        /// </summary>
        public void Disconnect()
        {
            if (Port.IsOpen)
                Port.Close();
        }
        
        /// <summary>
        /// Retorna um booleano indicando se a porta serial está aberta.
        /// </summary>
        public bool IsConnected()
        {
            return Port.IsOpen;
        }

        /// <summary>
        /// Retorna um booleano indicando se a conexão com o dispositivo é feito através de Bluetooth.
        /// </summary>
        public bool IsBluetooth()
        {
            return Bluetooth;
        }

        /// <summary>
        /// Retorna um booleano indicando se a classe está recebendo dados do dispositivo.
        /// </summary>
        public bool IsReceiving()
        {
            try
            {
                GetRaw();
                return true;
            }
            catch (IOException ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Reestabelece a conexão com a porta serial.
        /// </summary>
        public void Reconnect()
        {
            try
            {
                string name = Port.PortName;

                if (IsConnected())
                {
                    Disconnect();
                }

                if (Bluetooth)
                    this.Port = new SerialPort(name, 9600, Parity.None, 8, StopBits.One);
                else
                    this.Port = new SerialPort(name, 115200, Parity.None, 8, StopBits.One);

                this.Port.Handshake = Handshake.None;
                this.Port.ReadTimeout = 500;
                this.Port.WriteTimeout = 500;
                this.Port.DtrEnable = true;
                Connect();
            }
            catch (Exception ex)
            {
                // Não faz nada
            }
        }

        /// <summary>
        /// Obtém os dados de centro de gravidade e de massa do dispositivo.
        /// </summary>
        /// <remarks>Permite a compatibilidade com a versão antiga do dispositivo.</remarks>
        /// <returns>Retorna uma string no formato kg;x;y (Ex: 73.5;0.95;0.3).</returns>
        public string GetGravityData()
        {
            string[] point = GetData().Split('|');
            float x = float.Parse(point[0]);
            float y = float.Parse(point[1]);
            float z = float.Parse(point[2]);
            x *= 21f;
            y *= -12f;
            return z.ToString().Replace(".", ",") + ";" + x.ToString().Replace(".", ",") + ";" + y.ToString().Replace(".", ",");
        }

        /// <summary>
        /// Obtém os dados de centro de gravidade e de massa do dispositivo.
        /// </summary>
        /// <returns>Retorna uma string no formato x|y|kg (Ex: 73.5|0.95|0.3).</returns>
        public string GetData()
        {
            string[] point = GetPoint().Split('|');
            float x = float.Parse(point[0]);
            float y = float.Parse(point[1]);
            return ToSendFormat3(x, y, float.Parse(GetWeight()));
        }

        /// <summary>
        /// Obtém os dados de massa do dispositivo.
        /// </summary>
        /// <returns>Retorna uma string no formato kg (Ex: 73.5).</returns>
        public string GetWeight()
        {
            string[] weights = GetAllWeights().Split('|');
            float x = float.Parse(weights[0]);
            float y = float.Parse(weights[1]);
            float z = float.Parse(weights[2]);
            float w = float.Parse(weights[3]);
            float result = x + y + z + w;
            return result.ToString();
        }

        /// <summary>
        /// Obtém os dados de centro de gravidade do dispositivo.
        /// </summary>
        /// <returns>Retorna uma string no formato x|y (Ex: 0.95|0.3).</returns>
        public string GetPoint()
        {
            string[] weights = GetAllWeights().Split('|');
            float x = float.Parse(weights[0]);
            float y = float.Parse(weights[1]);
            float z = float.Parse(weights[2]);
            float w = float.Parse(weights[3]);
            float weight = x + y + z + w;
            float result_x = (y - x + w - z) / weight;
            float result_y = (z + w - x - y) / weight;

            if (AverageMass > 0)
            {
                if (result_y > AverageY)
                    result_y = ((result_y - AverageY) / (1 - AverageY)) * (weight / AverageMass);
                else
                    result_y = ((result_y - AverageY) / (1 + AverageY)) * (weight / AverageMass);

                if (result_x > AverageX)
                    result_x = ((result_x - AverageX) / (1 - AverageX)) * (weight / AverageMass);
                else
                    result_x = ((result_x - AverageX) / (1 + AverageX)) * (weight / AverageMass);
            }

            if (weight == 0)
            {
                result_x = 0;
                result_y = 0;
            }

            result_x = result_x * ConstantFactor;
            result_y = result_y * ConstantFactor;

            return ToSendFormat2(Mathf.Clamp(result_x, -1f, 1f), Mathf.Clamp(result_y, -1f, 1f));
        }

        /// <summary>
        /// Obtém os dados de massa em cada sensor do dispositivo.
        /// </summary>
        /// <returns>Retorna uma string no formato kg1|kg2|kg3|kg4 (Ex: 13.5|17.12|21.44|14.09).</returns>
        public string GetAllWeights()
        {
            return ToSendFormat4(Measurements[0], Measurements[1], Measurements[2], Measurements[3]);
        }

        /// <summary>
        /// Obtém os dados brutos do dispositivo.
        /// </summary>
        /// <returns>Retorna uma string no formato nometipobateria|kg1|kg2|kg3|kg4| (Ex: eboard0001c83%|13.5|17.12|21.44|14.09|).</returns>
        public string GetRaw()
        {
            return LastRead;
        }

        /// <summary>
        /// Formata duas floats para o formato padrão de transmissão.
        /// </summary>
        /// <returns>Retorna uma string no formato n1|n2 (Ex: 11.5|9.3).</returns>
        public string ToSendFormat2(float x, float y)
        {
            return x.ToString() + "|" + y.ToString();
        }

        /// <summary>
        /// Formata três floats para o formato padrão de transmissão.
        /// </summary>
        /// <returns>Retorna uma string no formato n1|n2|n3 (Ex: 11.5|9.3|15.2).</returns>
        public string ToSendFormat3(float x, float y, float z)
        {
            return x.ToString() + "|" + y.ToString() + "|" + z.ToString();
        }

        /// <summary>
        /// Formata quatro floats para o formato padrão de transmissão.
        /// </summary>
        /// <returns>Retorna uma string no formato n1|n2|n3|n4 (Ex: 11.5|9.3|15.2|21.1).</returns>
        public string ToSendFormat4(float x, float y, float z, float w)
        {
            return x.ToString() + "|" + y.ToString() + "|" + z.ToString() + "|" + w.ToString();
        }

        /// <summary>
        /// Lê os dados transmitidos pelo dispositivo e atualiza as propriedades da classe.
        /// </summary>
        public void Measure()
        {
            string result = "";

            if (!IsBluetooth())
            {
                result = Port.ReadLine();
            }
            else
            {
                int count = 0;
                bool started = false;

                while (Port.BytesToRead > 0 || count < 5)
                {
                    char c = (char)Port.ReadByte();
                    if (started)
                    {
                        result += c;
                        if (c == '\n' && count == 5)
                            break;
                        else if (c == '|')
                            count++;
                    }
                    if (c == 'e')
                    {
                        result += c;
                        started = true;
                    }
                }
            }

            string[] splitted = result.Split('|');

            if (result.Contains(Name.ToLower().Replace("-", "")) && splitted.Length >= 5 &&
                splitted[1] != null && splitted[2] != null &&
                splitted[3] != null && splitted[4] != null)
            {
                LastRead = result;

                Battery[MovingAveragePosition] = float.Parse(splitted[0].Replace(Name.ToLower().Replace("-", ""), "").Remove(0, 1).Replace("%", ""));
                MovingAveragePosition = (MovingAveragePosition + 1) % 50;

                float aux = 0;
                for (int i = 0; i < Battery.Length; i++)
                {
                    aux += Battery[i];
                }

                AverageBattery = aux / Battery.Length;
            }

            string[] raw = LastRead.Split('|');

            Measurements[0] = float.Parse(raw[1]);
            Measurements[1] = float.Parse(raw[2]);
            Measurements[2] = float.Parse(raw[3]);
            Measurements[3] = float.Parse(raw[4]);
        }

        /// <summary>
        /// Valor do nome do dispositivo.
        /// </summary>
        public string Name = "Eboard";

        /// <summary>
        /// Valor médio do eixo X do centro de massa do dispositivo.
        /// </summary>
        public float AverageX = 0;

        /// <summary>
        /// Valor médio do eixo Y do centro de massa do dispositivo.
        /// </summary>
        public float AverageY = 0;

        /// <summary>
        /// Valor médio da massa do dispositivo.
        /// </summary>
        public float AverageMass = 0;

        /// <summary>
        /// Valor médio da bateria do dispositivo.
        /// </summary>
        public float AverageBattery = 100;

        /// <summary>
        /// Indica se o aviso de bateria fraca do dispositivo já foi acionado.
        /// </summary>
        public bool LowBattery = false;

        /// <summary>
        /// Indica se esse dispositivo está se comunicando por Bluetooth.
        /// </summary>
        private bool Bluetooth = false;

        /// <summary>
        /// Fator que multiplica a leitura do centro de massa do dispositivo.
        /// </summary>
        private const float ConstantFactor = 1.15F;

        /// <summary>
        /// Array que armazena as leituras de cada sensor do dispositivo.
        /// </summary>
        private float[] Measurements = { 0, 0, 0, 0 };

        /// <summary>
        /// Leitura mais recente do dispositivo.
        /// </summary>
        private string LastRead = "eboard|0.000|0.000|0.000|0.000|";

        /// <summary>
        /// Posição atual do array de média móvel da bateria do dispositivo.
        /// </summary>
        private int MovingAveragePosition = 0;
        
        /// <summary>
        /// Array que armazena os valores mais recentes de bateria do dispositivo.
        /// </summary>
        private float[] Battery;

        /// <summary>
        /// Objeto da porta serial que faz conexão com o dispositivo.
        /// </summary>
        private SerialPort Port;
    }
}