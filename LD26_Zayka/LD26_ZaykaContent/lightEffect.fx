struct PS_input
{
	float4 color    : COLOR0;
	float2 texCoord : TEXCOORD0;
};

float lightRadius;
float2 lightpos;
float ambient;

Texture xTexture;
sampler TextureSampler = sampler_state { texture = <xTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};
sampler TextureSamplerReg0 : register(s0);
sampler TextureSamplerReg1 : register(s1);

// s0 - from spitebatch(s0...

float4 AlphaLights(PS_input PSIn):COLOR0
{
	float4 shadows = tex2D(TextureSamplerReg1, PSIn.texCoord);
	float4 this = tex2D(TextureSamplerReg0, PSIn.texCoord);
	float2 screenThis = PSIn.texCoord*float2(1024,768);

	float2 lightVector = lightpos - screenThis;
	
	float att = saturate(1.0-length(lightVector)/lightRadius-shadows.a);
	
	//lightVector = normalize(lightVector);

	return this = float4(this.xyz,this.a-att);
}

float4 AddAmbient(PS_input PSIn):COLOR0
{
	//float ambient=0.2;
	float4 this = tex2D(TextureSamplerReg0, PSIn.texCoord);	
	this.w-=ambient;
	return this;
	//return float4(0,1,0,0.5);
}

technique Lights
{
	pass Pass0
	{   		
		PixelShader  = compile ps_2_0 AlphaLights();
	}
}
technique Ambient
{
	pass Pass0
	{   		
		PixelShader  = compile ps_2_0 AddAmbient();
	}
}
