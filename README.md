# Loremaker
A fantasy lore generator useful for building fictional worlds. This project is still growing and currently only supports basic procedural text generation and map generation. Procedural name generation is mostly done through [Syllabore](https://github.com/kesac/Syllabore).

## Basic Text Generation

Use a TextTemplate to declare a string with substitutions as follows:

```C#
var text = new TextTemplate("{subject} {verb} to {place}.")
            .Define("subject", x => x.As("Alice", "Brian", "Cam"))
            .Define("verb", x => x.As("ran", "walked", "hopped"))
            .Define("place", x => x.As("store", "park", "house").UsingDeterminers("a", "the"));

for (int i = 0; i < 10; i++)
{
    Console.WriteLine(text.Next());
}
```

This generates text like:

```
Cam walked to the house.
Alice hopped to the park.
Brian hopped to a house.
```

A slightly more complicated example which includes adjectives:

```C#
var text = new TextTemplate("{subject} {verb} to {place}.")
            .CapitalizeFirstWord()
            .Define("subject", x => x
                .As("Alice", "Bob", "Chris")
                .UsingAdjectives("busy", "impatient", "energetic")
                .UsingDeterminers("the",""))
            .Define("verb", x => x
                .As("ran", "walked", "hopped"))
            .Define("place", x => x
                .As("store", "park", "house")
                .UsingAdjectives("green", "busy", "bustling")
                .UsingDeterminers("a", "the"));
```

This produces text like:
```
Energetic Alice walked to a bustling house.
Busy Bob hopped to a busy store.
The impatient Chris walked to the green park.
```

## Text Generation with Names

Loremaker supports the use of a Syllabore `NameGenerator` to generate names. Here is an example of a randomly generating place names in a piece of text:

```C#
 var text = new TextTemplate("{subject} {raised} in {birthplace} near {place}.")
            .Define("subject", x => x
                .As("Alice", "Brian", "Cam"))
            .Define("raised", x => x
                .As("grew up", "was raised", "was brought up"))
            .Define("birthplace", x => x
                .As("[village]", "[town]")
                .UsingAdjectives("modest", "poor", "busy", "remote", "trade", "coastal", "underground")
                .UsingDeterminers("a"))
            .Define("place", x => x
                .As("River", "Mountains", "Mountain Range", "Forest", "Ruins", "Sea", "Lake", "Plains")
                .UsingDeterminers("", "the")
                .UsingNamesFrom(new NameGenerator()
                    .UsingProvider(x => x
                        .WithVowels("aeo")
                        .WithLeadingConsonants("tvr"))));
```

The example above produces text like:
```
Brian was brought up in a poor town near Vara Forest.
Cam was brought up in a busy village near Teto Mountain Range.
Alice was brought up in a poor village near the Rota Plains.
```

## Generating Contextual Text
```C#
var chain = new TextChain()
            .Append("{subject} {raised} in {birthplace} near {place}.", x => x
                .Define("raised", x => x.As("grew up", "was raised", "was brought up"))
                .Define("birthplace", x => x
                    .As("[village]", "[town]") // square brackets marks context tags
                    .UsingAdjectives("modest", "poor", "trade", "coastal")
                    .UsingDeterminers("a"))
                .Define("place", x => x
                    .As("[Mountain] Range", "[Ocean]")
                    .UsingDeterminers("", "the")
                    .UsingNamesFrom(locationNames)))
            .Append("Growing up, {pronoun} was always drawn to the vastness of the ocean.", x => x
                .WhenContextHas("mountain")) // references an earlier context tag
            .Append("{pronoun} became a [sailor] at the age of {age}.", x => x
                .CapitalizeFirstWord()
                .Define("age", x => x.As("16", "17", "18"))
                .WhenContextHas("ocean")
                .AvoidWhenContextHas("mountain"))
            .DefineGlobally("world", x => x.UsingNamesFrom(worldNames))
            .DefineGlobally("subject", x => x.As("Alice", "Brian", "Cam"))
            .DefineGlobally("pronoun", x => x.As("he", "she"));
```

This will generate a chain of text like:

```
Cam was brought up in a coastal town near the Betai Mountain
Range. Growing up, he was always drawn to the vastness of
the ocean.
```

Or if the first sentence references an ocean, the text will be similar to this:

```
Alice was brought up in a modest village near the Cebuti
Ocean. She became a sailor at the age of 16.
```


## License

MIT License

Copyright (c) 2019-2021 Kevin Sacro

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
