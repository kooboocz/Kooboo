import { getAllElement, getImportant, clearCssImageWarp } from "@/dom/utils";
import { setGuid } from "@/kooboo/utils";
import { KOOBOO_ID, BACKGROUND_IMAGE_START } from "@/common/constants";
import { setImagePreview } from "./utils";
import { pickImg } from "@/kooboo/outsideInterfaces";
import { operationRecord } from "@/operation/Record";
import context from "@/common/context";
import { AttributeUnit } from "@/operation/recordUnits/attributeUnit";
import { KoobooComment } from "@/kooboo/KoobooComment";
import { createDiv } from "@/dom/element";
import { createImagePreview } from "../common/imagePreview";
import { getCssRules, CssRule, matchSelector } from "@/dom/style";
import { BgImageUnit } from "@/operation/recordUnits/bgImageUnit";
import { getEditableComment } from "../floatMenu/utils";
import { kvInfo } from "@/common/kvInfo";
import { Log } from "@/operation/Log";

export function createStyleImagePanel() {
  let contiainer = createDiv();
  let rules = getCssRules().filter(f => f.cssRule.style.backgroundImage && BACKGROUND_IMAGE_START.test(f.cssRule.style.backgroundImage));
  let appendedRule: matchedRule[] = [];

  for (const element of getAllElement(document.body)) {
    if (!(element instanceof HTMLElement)) continue;
    let style = getComputedStyle(element);
    if (!style.backgroundImage || style.backgroundImage == "none") continue;
    let matchedRules = getRule(element, rules);
    if (matchedRules.length > 0) {
      for (const matchedRule of matchedRules) {
        let styleImagePreview = createStyleImagePreview(appendedRule, element, matchedRule);
        if (styleImagePreview) contiainer.appendChild(styleImagePreview);
      }
    } else if (element.style.backgroundImage) {
      let inlineImagePreview = createInlineImagePreview(element, style, rules);
      if (inlineImagePreview) contiainer.appendChild(inlineImagePreview);
    }
  }

  return contiainer;
}

function getRule(el: HTMLElement, rules: CssRule[]) {
  let matchedRules: matchedRule[] = [];

  for (const rule of rules) {
    for (const i of matchSelector(el, rule.cssRule.selectorText)) {
      matchedRules.push({
        selector: i.selector,
        pseudo: i.pseudo,
        rule: rule
      });
    }
  }

  return matchedRules;
}

interface matchedRule {
  selector: string;
  pseudo: string;
  rule: CssRule;
}

function createInlineImagePreview(element: HTMLElement, style: CSSStyleDeclaration, rules: CssRule[]) {
  let koobooId = element.getAttribute(KOOBOO_ID);
  let comments = KoobooComment.getComments(element);
  let comment = getEditableComment(comments)!;
  if (!comment || !koobooId) return;
  let { imagePreview, setImage } = createImagePreview();
  setImagePreview(imagePreview, element);
  let src = clearCssImageWarp(style.backgroundImage!);
  if (!src) return;
  setImage(src);
  imagePreview.onclick = () => {
    let startContent = element.getAttribute("style");
    pickImg(path => {
      path = path == "none" ? "none" : ` url('${path}')`;
      let important = getImportant(element, "background-image", rules);
      element.style.setProperty("background-image", path, important);
      setImage(path);
      let guid = setGuid(element);
      let unit = new AttributeUnit(startContent!, "style");
      let value = element.style.backgroundImage!.replace(/"/g, "'");
      let log = [...comment.infos, kvInfo.value(value), kvInfo.property("background-image"), kvInfo.koobooId(koobooId), kvInfo.important(important)];
      let record = new operationRecord([unit], [new Log(log)], guid);
      context.operationManager.add(record);
    });
  };
  return imagePreview;
}

function createStyleImagePreview(appendedRule: matchedRule[], element: HTMLElement, matchedRule: matchedRule) {
  if (appendedRule.some(s => s.pseudo == matchedRule.pseudo && s.selector == matchedRule.selector && s.rule == matchedRule.rule)) return;
  appendedRule.push(matchedRule);
  let rule = matchedRule.rule;
  let { imagePreview, setImage } = createImagePreview();
  setImagePreview(imagePreview, element);
  let src = clearCssImageWarp(rule.cssRule.style.backgroundImage!);
  if (!src) return;
  setImage(src);
  imagePreview.onclick = () => {
    let startContent = rule.cssRule.style.backgroundImage;
    pickImg(path => {
      path = path == "none" ? "none" : ` url('${path}')`;
      let important = rule.cssRule.style.getPropertyPriority("background-image");
      rule.cssRule.style.setProperty("background-image", path, important);
      setImage(path);
      let guid = setGuid(element);
      let unit = new BgImageUnit(startContent!, rule.cssRule.style);
      let value = rule.cssRule.style.backgroundImage!.replace(/"/g, "'");
      let log = [
        kvInfo.value(value),
        kvInfo.property("background-image"),
        new kvInfo("selector", rule.cssRule.selectorText),
        kvInfo.koobooId(rule.url ? "" : rule.koobooId),
        kvInfo.important(important),
        kvInfo.mediaRuleList(rule.mediaRuleList!)
      ];
      let record = new operationRecord([unit], [new Log(log)], guid);
      context.operationManager.add(record);
    });
  };
  return imagePreview;
}
