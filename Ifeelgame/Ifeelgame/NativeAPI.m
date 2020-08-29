//
//  NativeAPI.m
//  Ifeelgame
//
//  Created by ifeel on 2020/8/27.
//  Copyright © 2020 ifeel. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "NativeAPI.h"

@implementation NativeAPI

static NativeAPI *_instance = nil;

+(NativeAPI*) Instance{
    static dispatch_once_t once;
    dispatch_once(&once, ^{
        NSLog(@"Creating NativeAPI Instance");
        _instance = [[NativeAPI alloc] init];
    });
    return _instance;
}

-(id)init {
    self = [super init];
    return self;
}

-(const char*) GetRegionName{
    NSLocale *currentLocale = [NSLocale currentLocale];
    NSString *countryCode = [currentLocale objectForKey:NSLocaleCountryCode];
    return [[countryCode uppercaseString] UTF8String];
}

@end
