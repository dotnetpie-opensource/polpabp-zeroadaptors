﻿<h3>{{L "EmailActivation_Title"}}</h3>
<h4>{{L "EmailActivation_SubTitle"}}</h4>

<p>
    <b>{{L "TenancyName"}}</b>: <span>{{model.tenancy}}</span>
</p>

<div>
    <a href="{{model.link}}">{{L "EmailActivation_ClickTheLinkBelowToVerifyYourEmail"}}</a>
    <p>
       {{L "EmailMessage_CopyTheLinkBelowToYourBrowser"}} <span>{{model.link}}</span>
    </p>
</div>