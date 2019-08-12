using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropzoneController : MonoBehaviour {

    public int _actionType = 0;
    public int _fractionId = 0;
    public int _playerId = 0;
    public bool _permDropZone = false;

    public Image _dropzoneMarkBackground;
    public Color _originalColor;
    public Color _ableColor;
    public Color _unableColor;

    public void MarkAbleDropzone () {
        _originalColor = _dropzoneMarkBackground.color;
        _dropzoneMarkBackground.color = _ableColor;
    }

    public void MarkUnableDropzone () {
        _originalColor = _dropzoneMarkBackground.color;
        _dropzoneMarkBackground.color = _unableColor;
    }

    public void UnarkDropzone () {
        _dropzoneMarkBackground.color = _originalColor;
    }
}