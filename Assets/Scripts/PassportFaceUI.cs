using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     The passport face as a normal UI layout. Fills the icons and date from a Passport.
///     Only knows about UI, PassportView decides how it ends up on the mesh.
///     Expects a fresh instance every time, it doesn't clear icons from a previous fill.
/// </summary>
public class PassportFaceUI : MonoBehaviour
{
    [Tooltip("Where the icons get spawned, put a layout group on it.")] [SerializeField]
    private RectTransform iconParent;

    [Tooltip("Disabled child that gets copied once per required item.")] [SerializeField]
    private Image iconTemplate;

    [SerializeField] private TMP_Text dateText;

    /// <summary>
    ///     Puts one icon per required item and writes the date.
    /// </summary>
    /// <param name="passport">The passport to show</param>
    public void SetPassport(Passport passport)
    {
        iconTemplate.gameObject.SetActive(false);
        foreach (var item in passport.Required)
        {
            var icon = Instantiate(iconTemplate, iconParent);
            icon.name = $"Icon_{item.name}";
            icon.sprite = item.icon;
            icon.gameObject.SetActive(true);
        }

        dateText.text = FormatDate(passport.BirthDate);
    }

    // TODO(Cole) I will go and make this display month names and more fun stuff later
    private static string FormatDate(PassportDate date)
    {
        return $"Born {date}";
    }
}