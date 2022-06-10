#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

matrix WorldViewProjection;
float2 size;
float2 table;
Texture2D tex;
float4 backcolor;
sampler2D textSamp = sampler_state
{
	Texture = <tex>;
};


struct VertexShaderInput
{
	float4 pos : POSITION0;
	float4 col : COLOR0;
	float2 tex : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 pos : SV_POSITION;
	float4 col : COLOR0;
	float2 tex : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.pos = mul(input.pos, WorldViewProjection);
	output.col = input.col;
	output.tex = input.tex;
	return output;
}

float sdSquircle(float2 pos)
{
	float x = pos.x;
	float y = pos.y;
	x *= x;
	y *= y;
	return pow(x * x + y * y, .25);
}

float smin(float d1, float d2, float k)
{
	//return min(d1, d2);
	float h = clamp(.5 + .5 * (d2 - d1) / k, 0., 1.);
	return lerp(d2, d1, h) - k * h * (1. - h);
}
float ssub(float d2, float d1, float k)
{
	//return max(d1, -d2);
	float h = clamp(.5 - .5 * (d2 + d1) / k, 0., 1.);
	return lerp(d2, -d1, h) + k * h * (1. - h);
}

float normalize(float value, float minv, float maxv, float newMin, float newMax)
{
	return (value - minv) * ((newMax - newMin) / (maxv - minv)) + newMin;
}

float sdBox(in float2 p, in float2 b)
{
	float2 d = abs(p) - b;
	return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0);
}

float mask(float2 pos, int id, int side)
{
	float dist = .3;
	float d = length(pos.x) - dist;
	d = max(d, (length(pos.y) - dist));
	float smooth = .05;
	if ((side & (1 << 0)) == 0) {
		if ((id & (1 << 0)) != 0)
			//d = min(d, (length(pos + float2(dist + .04, .0)) - .1));
			d = smin(d, (length(pos + float2(dist + .04, .0)) - .1), smooth);
		else if ((id & (1 << 0)) == 0)
			d = ssub(d, (length(pos + float2(dist - .04, .0)) - .1), smooth);
		//d = max(d, -(length(pos + float2(dist - .04, .0)) - .1));
	}
	if ((side & (1 << 1)) == 0) {
		if ((id & (1 << 1)) != 0)
			d = smin(d, (length(pos + float2(.0, dist + .04)) - .1), smooth);
		else if ((id & (1 << 1)) == 0)
			d = ssub(d, (length(pos + float2(.0, dist - .04)) - .1), smooth);
	}
	if ((side & (1 << 2)) == 0) {
		if ((id & (1 << 2)) != 0)
			d = smin(d, (length(pos + float2(-(dist + .04), .0)) - .1), smooth);
		else if ((id & (1 << 2)) == 0)
			d = ssub(d, (length(pos + float2(-(dist - .04), .0)) - .1), smooth);
	}
	if ((side & (1 << 3)) == 0) {
		if ((id & (1 << 3)) != 0)
			d = smin(d, (length(pos + float2(.0, -(dist + .04))) - .1), smooth);
		else if ((id & (1 << 3)) == 0)
			d = ssub(d, (length(pos + float2(.0, -(dist - .04))) - .1), smooth);
	}

	float radius = 0.1;
	if ((side & (1 << 0)) != 0 && (side & (1 << 1)) != 0) // lt	
		d = max(d, sdBox(pos + float2(-.3, -.3), .6 - radius) - radius);
		//d = max(d, length(pos + float2(-.3, -.3)) - .6);

	if ((side & (1 << 0)) != 0 && (side & (1 << 3)) != 0) // lb
		d = max(d, sdBox(pos + float2(-.3, +.3), .6 - radius) - radius);		

	if ((side & (1 << 2)) != 0 && (side & (1 << 1)) != 0) // rt	
		d = max(d, sdBox(pos + float2(+.3, -.3), .6 - radius) - radius);		

	if ((side & (1 << 2)) != 0 && (side & (1 << 3)) != 0) // rb	
		d = max(d, sdBox(pos + float2(+.3, +.3), .6 - radius) - radius);




	return d;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float2 uv = input.tex;// -float2(.2, .2) / float2(7., 5.);
	float2 pos = frac(uv * table) - .5; // frac((input.tex / size)* float2(7., 5.));

	int id = int(input.col.r * 255);
	int side = int(input.col.g * 255);

	float d = mask(pos, id, side);

	float s = smoothstep(.01, .0, d);
	//uv = (input.tex - (float2(.2, .2) / float2(7., 5.)));// / (1 / .6);
	//uv /= .6;

	uv = input.tex * table;
	float2 iuv = floor(uv);
	float2 fuv = frac(uv);

	fuv.x = normalize(fuv.x, .0, 1., -.33333, 1.33333);
	fuv.y = normalize(fuv.y, .0, 1., -.33333, 1.33333);
	uv = iuv + fuv;
	uv /= table;

	float4 col = tex2D(textSamp, uv);// * input.col;
	//col.rb = pos;
	col.a *= s;
	//col.a += .1;
	//if (uv.x < 0 || uv.y < 0) col = 1.;
	//if (uv.x > 1 || uv.y > 1) col = 1.;
	return col;
}

float4 backPS(VertexShaderOutput input) : COLOR
{
	float2 uv = input.tex;
	float2 pos = frac(uv * table) - .5;

	int id = int(input.col.r * 255);
	int side = int(input.col.g * 255);

	float d = mask(pos, id, side);

	float s = smoothstep(.01, .0, d);	

	float4 col = backcolor; // tex2D(textSamp, uv);// * input.col;
	col.a *= s;
	
	return col;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL backPS();
	}
};