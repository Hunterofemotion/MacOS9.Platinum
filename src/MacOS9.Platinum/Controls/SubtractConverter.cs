using System;
using System.Globalization;
using System.Windows.Data;

namespace MacOS9.Platinum.Controls;

/// <summary>
/// Resta un número fijo a una medida. Nunca devuelve menos de cero.
/// </summary>
/// <remarks>
/// Existe por un caso concreto: el título de un recuadro de agrupación tiene que
/// caber dejando sitio al marco de su derecha, y eso es el ancho del recuadro
/// menos una cantidad. Un Grid no encoge una columna Auto para que quepa —la deja
/// desbordar—, así que el tope va sobre el ancho del propio título y no hay forma
/// de expresar la resta en el marcado sin esto.
/// </remarks>
public sealed class SubtractConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not double medida || double.IsNaN(medida) || double.IsInfinity(medida))
        {
            return double.PositiveInfinity;
        }

        double resta = 0;
        if (parameter is not null)
        {
            double.TryParse(System.Convert.ToString(parameter, CultureInfo.InvariantCulture),
                NumberStyles.Float, CultureInfo.InvariantCulture, out resta);
        }

        return Math.Max(0d, medida - resta);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException("La resta no se deshace: solo sirve para medir.");
}
