//
//  MockRandomNumberGenerator.swift
//  Sudoku
//
//  Created by Christopher Shipstone on 30/03/2026.
//

internal struct MockRandomNumberGenerator : RandomNumberGenerator {
    internal var nextClosure: () -> UInt64
    
    internal init() {
        self.nextClosure = { 0 }
    }
    
    internal func next() -> UInt64 {
        self.nextClosure()
    }
}
