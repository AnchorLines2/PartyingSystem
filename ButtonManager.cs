using UnityEngine;
using UnityEngine.UIElements;

public class ButtonManager
{
    public static Button AddHoverEffectsForButton(Button button)
    {
        button.RegisterCallback<MouseEnterEvent>(new EventCallback<MouseEnterEvent>((evt) =>
        {
            button.style.backgroundColor = Color.white;
            button.style.color = Color.black;
        }));
        button.RegisterCallback<MouseLeaveEvent>(new EventCallback<MouseLeaveEvent>((evt) =>
        {
            button.style.backgroundColor = new StyleColor(new Color(0.25f, 0.25f, 0.25f));
            button.style.color = Color.white;
        }));
        return button;
    }

    public static Button createLobbyButton(Button referenceButton)
    {
        Button button = new Button
        {
            text = "LOBBY PRACTICE - 2",
            style =
                    {
                        backgroundColor = new StyleColor(new Color(0.25f, 0.25f, 0.25f)),
                        unityTextAlign = TextAnchor.MiddleLeft,
                        width = new StyleLength(new Length(100, LengthUnit.Percent)),
                        minWidth = new StyleLength(new Length(100, LengthUnit.Percent)),
                        maxWidth = new StyleLength(new Length(100, LengthUnit.Percent)),
                        height = referenceButton.style.height,
                        minHeight = referenceButton.style.minHeight,
                        maxHeight = referenceButton.style.maxHeight,
                        marginTop = 8,
                        paddingTop = 8,
                        paddingBottom = 8,
                        paddingLeft = 15
                    }
        };
        AddHoverEffectsForButton(button);
        return button;
    }
    
    public static void addRankedButton(Button mainMenuSettingsButton, EventCallback<ClickEvent> function)
    {
        VisualElement containerVisualElement = mainMenuSettingsButton.parent;
        if (containerVisualElement == null)
        {
            Debug.LogError("Container VisualElement not found (parent of settingsButton missing)!");
            return;
        }

        Button reskinMenuButton = ButtonManager.createLobbyButton(mainMenuSettingsButton);
        reskinMenuButton.RegisterCallback<ClickEvent>(function);
        containerVisualElement.Insert(2, reskinMenuButton);
    }
}
