//
//  randomArray.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

import Testing

internal func randomArray() -> [Int] {
    var array: [Int] = []
    var generator = SystemRandomNumberGenerator()
    
    for _ in 0..<81 {
        let value = Int.random(in: 1...9, using: &generator)
        array.append(value)
    }
    
    return array
}
