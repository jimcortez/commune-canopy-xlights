
/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "boolInput",
			"TYPE": "bool",
			"DEFAULT": 1.0
		},
		{
			"NAME": "colorInput",
			"TYPE": "color",
			"DEFAULT": [
				0.0,
				0.0,
				1.0,
				1.0
			]
		},
		{
			"NAME": "flashInput",
			"TYPE": "event"
		},
		{
			"NAME": "floatInput",
			"TYPE": "float",
			"DEFAULT": 0.5,
			"MIN": 0.0,
			"MAX": 1.0
		},
		{
			"NAME": "longInputIsAPopUpButton",
			"TYPE": "long",
			"VALUES": [
				0,
				1,
				2
			],
			"LABELS": [
				"red",
				"green",
				"blue"
			],
			"DEFAULT": 1
		},
		{
			"NAME": "pointInput",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				0
			]
		}
	],
	"PASSES": [
		{
			"TARGET":"bufferVariableNameA",
			"WIDTH": "$WIDTH/16.0",
			"HEIGHT": "$HEIGHT/16.0"
		},
		{
			"DESCRIPTION": "this empty pass is rendered at the same rez as whatever you are running the ISF filter at- the previous step rendered an image at one-sixteenth the res, so this step ensures that the output is full-size"
		}
	]
	
}*/

/*
 * Copyright 2014 Roman Bobniev (FatumR)
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#define OCTAVES  8.0

#define LIVE_SMOKE 0

#define iTime (TIME/2.0)   // adjust divisor to speed up or slow down
#define iResolution RENDERSIZE


float rand(vec2 co){
   return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

float rand2(vec2 co){
   return fract(cos(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

// Rough Value noise implementation
float valueNoiseSimple(vec2 vl) {
   float minStep = 1.0 ;

   vec2 grid = floor(vl);
   vec2 gridPnt1 = grid;
   vec2 gridPnt2 = vec2(grid.x, grid.y + minStep);
   vec2 gridPnt3 = vec2(grid.x + minStep, grid.y);
   vec2 gridPnt4 = vec2(gridPnt3.x, gridPnt2.y);

    float s = rand2(grid);
    float t = rand2(gridPnt3);
    float u = rand2(gridPnt2);
    float v = rand2(gridPnt4);
    
    float x1 = smoothstep(0., 1., fract(vl.x));
    float interpX1 = mix(s, t, x1);
    float interpX2 = mix(u, v, x1);
    
    float y = smoothstep(0., 1., fract(vl.y));
    float interpY = mix(interpX1, interpX2, y);
    
    return interpY;
}

float getLowFreqs()
{
    const int NUM_FREQS = 32;
    /* Close to the spectrum of the voice frequencies for this song. */
    const float lowStart = 0.65;
    const float lowEnd = 0.75;
    float result = 0.0;
    
    for (int i = 0; i < NUM_FREQS; i++)
    {
//        result += texture(iChannel0,
//                            vec2(lowStart + (lowEnd - lowStart)*float(i)/float(NUM_FREQS - 1),
//                                 0.25)).x;
    }


    result = sin(iTime/12.0);

    return smoothstep(0.0, 1.0, (result / float(NUM_FREQS)) * 2.);
}

float fractalNoise(vec2 vl) {
    float persistance = 1.8;
    float amplitude = 0.5;
    float rez = 0.0;
    vec2 p = vl;
    
    for (float i = 0.0; i < OCTAVES; i++) {
        rez += amplitude * valueNoiseSimple(p);
        amplitude /= persistance;
        p *= persistance;
    }
    return rez;
}

float complexFBM(vec2 p) {
    float sound = getLowFreqs();
    float slow = iTime / 2.5;
    float fast = iTime / .5;
    vec2 offset1 = vec2(slow  , 0.); // Main front
    vec2 offset2 = vec2(sin(fast )* 0.1, 0.); // sub fronts

    return 
#if LIVE_SMOKE
        (1. + sound) * 
#endif
        fractalNoise( p + offset1 + fractalNoise(
            	p + fractalNoise(
                	p + 2. * fractalNoise(p - offset2)
            	)
        	)
        );
}


void main()
{
    vec2 uv = gl_FragCoord.xy / iResolution.xy;
   
//    vec3 blueColor = vec3(0.529411765, 0.807843137, 0.980392157);
//    vec3 orangeColor2 = vec3(0.509803922, 0.203921569, 0.015686275);
    vec3 blueColor = vec3(2.0/255.0, 29.0/255.0, 135.0/255.0);
    vec3 orangeColor2 = vec3(0.86, 0.655, 0.2);

    
    vec3 rez = mix(orangeColor2, blueColor, complexFBM(uv));
    
    gl_FragColor = vec4(rez, 1.0);
    
}

