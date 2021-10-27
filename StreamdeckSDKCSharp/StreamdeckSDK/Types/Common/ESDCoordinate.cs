namespace Elgato.StreamdeckSDK.Types.Common
{
    public class ESDCoordinate
    {
        public int Column { get; set; }
        public int Row { get; set; }

        public override string ToString()
        {
            return $"({Column}/{Row})";
        }
    }
}
