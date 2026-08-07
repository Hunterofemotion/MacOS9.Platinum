using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MacOS9.Platinum.Controls;

/// <summary>
/// Las flechitas de Mac OS 9: un par de teclas apiladas que suben o bajan el valor
/// del campo de al lado. Son un control aparte y no parte del campo porque el
/// sistema las trataba así —servían para cualquier valor, no solo fechas— y porque
/// mantienen la repetición al dejar el botón oprimido.
///
/// No guarda ningún valor: solo avisa hacia dónde se pidió mover. Quien la use
/// decide qué significa un paso.
/// </summary>
[TemplatePart(Name = PartUp, Type = typeof(ButtonBase))]
[TemplatePart(Name = PartDown, Type = typeof(ButtonBase))]
public class PlatinumStepper : Control
{
    public const string PartUp = "PART_Up";
    public const string PartDown = "PART_Down";

    static PlatinumStepper()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(PlatinumStepper), new FrameworkPropertyMetadata(typeof(PlatinumStepper)));
    }

    /// <summary>Se pidió un paso. El signo dice hacia dónde.</summary>
    public event EventHandler<int>? Stepped;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild(PartUp) is ButtonBase arriba)
        {
            arriba.Click += (_, _) => Stepped?.Invoke(this, 1);
        }
        if (GetTemplateChild(PartDown) is ButtonBase abajo)
        {
            abajo.Click += (_, _) => Stepped?.Invoke(this, -1);
        }
    }
}
