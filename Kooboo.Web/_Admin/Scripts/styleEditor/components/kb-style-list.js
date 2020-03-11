Vue.component("kb-style-list", {
  template: Kooboo.getTemplate(
    "/_Admin/Scripts/styleEditor/components/kb-style-list.html"
  ),
  props: {
    rules: {
      type: Array,
      default: []
    },
    showRelationBtn: Boolean
  },
  data: function() {
    return {
      colorProperty: [
        "color",
        "background-color",
        "border",
        "border-top",
        "border-right",
        "border-bottom",
        "border-left",
        "border-color",
        "border-top-color",
        "border-right-color",
        "border-bottom-color",
        "border-left-color",
        "box-shadow",
        "text-shadow",
        "outline",
        "outline-color"
      ],
      imageProperty: ["background-image"],
      colorAndImageProperty: ["background"],
      propertyName: [
        "accelerator",
        "azimuth",
        "background",
        "background-attachment",
        "background-color",
        "background-image",
        "background-position",
        "background-position-x",
        "background-position-y",
        "background-repeat",
        "behavior",
        "border",
        "border-bottom",
        "border-bottom-color",
        "border-bottom-style",
        "border-bottom-width",
        "border-collapse",
        "border-color",
        "border-left",
        "border-left-color",
        "border-left-style",
        "border-left-width",
        "border-right",
        "border-right-color",
        "border-right-style",
        "border-right-width",
        "border-spacing",
        "border-style",
        "border-top",
        "border-top-color",
        "border-top-style",
        "border-top-width",
        "border-width",
        "bottom",
        "caption-side",
        "clear",
        "clip",
        "color",
        "content",
        "counter-increment",
        "counter-reset",
        "cue",
        "cue-after",
        "cue-before",
        "cursor",
        "direction",
        "display",
        "elevation",
        "empty-cells",
        "filter",
        "float",
        "font",
        "font-family",
        "font-size",
        "font-size-adjust",
        "font-stretch",
        "font-style",
        "font-variant",
        "font-weight",
        "height",
        "ime-mode",
        "include-source",
        "layer-background-color",
        "layer-background-image",
        "layout-flow",
        "layout-grid",
        "layout-grid-char",
        "layout-grid-char-spacing",
        "layout-grid-line",
        "layout-grid-mode",
        "layout-grid-type",
        "left",
        "letter-spacing",
        "line-break",
        "line-height",
        "list-style",
        "list-style-image",
        "list-style-position",
        "list-style-type",
        "margin",
        "margin-bottom",
        "margin-left",
        "margin-right",
        "margin-top",
        "marker-offset",
        "marks",
        "max-height",
        "max-width",
        "min-height",
        "min-width",
        "-moz-binding",
        "-moz-border-radius",
        "-moz-border-radius-topleft",
        "-moz-border-radius-topright",
        "-moz-border-radius-bottomright",
        "-moz-border-radius-bottomleft",
        "-moz-border-top-colors",
        "-moz-border-right-colors",
        "-moz-border-bottom-colors",
        "-moz-border-left-colors",
        "-moz-opacity",
        "-moz-outline",
        "-moz-outline-color",
        "-moz-outline-style",
        "-moz-outline-width",
        "-moz-user-focus",
        "-moz-user-input",
        "-moz-user-modify",
        "-moz-user-select",
        "orphans",
        "outline",
        "outline-color",
        "outline-style",
        "outline-width",
        "overflow",
        "overflow-X",
        "overflow-Y",
        "padding",
        "padding-bottom",
        "padding-left",
        "padding-right",
        "padding-top",
        "page",
        "page-break-after",
        "page-break-before",
        "page-break-inside",
        "pause",
        "pause-after",
        "pause-before",
        "pitch",
        "pitch-range",
        "play-during",
        "position",
        "quotes",
        "-replace",
        "richness",
        "right",
        "ruby-align",
        "ruby-overhang",
        "ruby-position",
        "-set-link-source",
        "size",
        "speak",
        "speak-header",
        "speak-numeral",
        "speak-punctuation",
        "speech-rate",
        "stress",
        "scrollbar-arrow-color",
        "scrollbar-base-color",
        "scrollbar-dark-shadow-color",
        "scrollbar-face-color",
        "scrollbar-highlight-color",
        "scrollbar-shadow-color",
        "scrollbar-3d-light-color",
        "scrollbar-track-color",
        "table-layout",
        "text-align",
        "text-align-last",
        "text-decoration",
        "text-indent",
        "text-justify",
        "text-overflow",
        "text-shadow",
        "text-transform",
        "text-autospace",
        "text-kashida-space",
        "text-underline-position",
        "top",
        "unicode-bidi",
        "-use-link-source",
        "vertical-align",
        "visibility",
        "voice-family",
        "volume",
        "white-space",
        "widows",
        "width",
        "word-break",
        "word-spacing",
        "word-wrap",
        "writing-mode",
        "z-index",
        "zoom"
      ],
      styleRules: []
    };
  },
  mounted: function() {
    var self = this;
    self.setStyleRules();
  },
  methods: {
    removeSelector: function() {},
    showRelation: function() {},
    addDeclaration: function(rule) {
      rule.declarations.push({
        name: "",
        value: "",
        important: false,
        showDecName: true,
        showDecValue: false,
        btns: false,
        inputValue: ""
      });
    },
    appendImportant: function(dec) {
      dec.inputValue = dec.value + (dec.important ? " !important" : "");
      this.$forceUpdate();
    },
    ableToShowColorPicker: function(dec) {
      return (
        this.colorProperty.indexOf(dec.name) > -1 ||
        this.colorAndImageProperty.indexOf(dec.name) > -1
      );
    },
    onPickImageBtnClick: function(dec) {
      Kooboo.EventBus.publish("ko/style/list/pickimage/show", dec);
    },
    ableToShowChangeImgBtn: function(dec) {
      return (
        this.imageProperty.indexOf(dec.name) > -1 ||
        this.colorAndImageProperty.indexOf(dec.name) > -1
      );
    },
    decNameInputBlur: function(rule, dec) {
      dec.showDecName = false;
      if (!dec.name) {
        rule.declarations = rule.declarations.filter(function(f) {
          return f != dec;
        });
      }
      this.$forceUpdate();
    },
    decValueInputBlur: function(dec) {
      dec.showDecValue = false;
      var valueArr = dec.inputValue.match(/([\w_/\?]*)( !important)?$/);

      if (valueArr[2]) {
        dec.important = true;
        dec.value = valueArr[1];
      } else {
        dec.value = dec.inputValue;
        dec.important = false;
      }

      this.$forceUpdate();
    },
    switchToValueInput: function(dec) {
      var self = this;
      setTimeout(function() {
        dec.showDecName = false;
        dec.showDecValue = true;
        self.$forceUpdate();
      }, 100);
    },
    onShowSelectorInput: function(rule) {
      rule.showSelectorInput = true;
      this.$forceUpdate();
    },
    setStyleRules: function() {
      var self = this;
      self.styleRules = self.rules.map(function(rule) {
        rule.showSelectorInput = false;
        rule.declarations = rule.declarations.map(function(dec) {
          dec.name = dec.propertyName;
          dec.showDecName = !dec.name;
          dec.showDecValue = false;
          dec.btns = false;
          dec.inputValue = "";
          return dec;
        });
        return rule;
      });
    },
    ruleBlur: function(rule) {
      var self = this;
      rule.showSelectorInput = false;
      if (!rule.selector) {
        self.styleRules = self.styleRules.filter(function(f) {
          return f != rule;
        });
      }
      this.$forceUpdate();
    }
  }
});
